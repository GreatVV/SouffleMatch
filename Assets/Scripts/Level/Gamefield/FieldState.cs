#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#endregion

[Serializable]
public class FieldState : GamefieldState
{
    #region Direction enum

    public enum Direction
    {
        Right,
        Left,
        Top,
        Bottom
    };

    #endregion

    public Chuzzle CurrentChuzzle;
    public Vector3 CurrentChuzzlePrevPosition;
    public Direction CurrentDirection;
    public GameObject DownArrow;
    public List<Chuzzle> SelectedChuzzles = new List<Chuzzle>();
    public float TimeFromTip = 0;
    private bool _axisChozen;

    private Vector3 _delta;
    private Vector3 _deltaTouch;
    private Vector3 _dragOrigin;
    private Chuzzle _draggable;
    private bool _isVerticalDrag;
    private float _maxX;
    private float _maxY;

    private float _minX;
    private float _minY;
    private bool _isReturning;
    private bool _hasLockedChuzzles;

    public bool IsTurn = false;

    #region Event Handlers

    public override void OnEnter()
    {
        AnimatedChuzzles.Clear();
        Chuzzle.DropEventHandlers();
        Chuzzle.AnimationStarted += OnAnimationStarted;
        if (IsTurn)
        {
            InvaderChuzzle.Populate(Gamefield);
        }
        IsTurn = true;
    }

    public override void OnExit()
    {
        if (AnimatedChuzzles.Any())
        {
            Debug.LogError("FUCK you in field state: " + AnimatedChuzzles.Count);
        }
        Gamefield.GameMode.HumanTurn();
    }

    public void CheckCombinations()
    {
        var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
        if (combinations.Any())
        {
            foreach (var c in Gamefield.Level.Chuzzles)
            {
                c.MoveTo = c.Current = c.Real;
            }
            Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);         
            Reset();
        }
        else
        {
            if (CurrentChuzzle != null)
            {
                var velocity = -5f*(CurrentChuzzle.Real.Position - CurrentChuzzle.Current.Position);
                foreach (var c in SelectedChuzzles)
                {
                    c.MoveTo = c.Real = c.Current;
                    c.Velocity = velocity;
                }

                _isReturning = true;
            }
        }
    }

    private void OnAnimationFinished(Chuzzle chuzzle)
    {
        chuzzle.AnimationFinished -= OnAnimationFinished;
        AnimatedChuzzles.Remove(chuzzle);

        CheckAnimationCompleted();
    }

    private void OnAnimationStarted(Chuzzle chuzzle)
    {
        if (!AnimatedChuzzles.Contains(chuzzle))
        {
            AnimatedChuzzles.Add(chuzzle);
            chuzzle.AnimationFinished += OnAnimationFinished;
        }
    }

    #endregion

    public void UpdateState(IEnumerable<Chuzzle> draggableChuzzles)
    {
        if (_isReturning)
        {
            return;
        }


        TimeFromTip += Time.deltaTime;
        if (TimeFromTip > 1 && !Gamefield.Level.ActiveChuzzles.Any(x => x.Shine))
        {
            IntVector2 targetPosition = null;
            Chuzzle arrowChuzzle = null;
            var possibleCombination = GamefieldUtility.Tip(Gamefield.Level.ActiveChuzzles, out targetPosition,
                out arrowChuzzle);
            if (possibleCombination.Any())
            {
                foreach (var chuzzle in possibleCombination)
                {
                    chuzzle.Shine = true;
                }
                GamefieldUtility.ShowArrow(arrowChuzzle, targetPosition, DownArrow);
            }
            else
            {
                RepaintRandom();
                return;
            }


            TimeFromTip = 0;
        }

        #region Drag

        if (CurrentChuzzle == null &&
            (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            _dragOrigin = Input.mousePosition;
            Debug.Log("Position: " + _dragOrigin);

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //   Debug.Log("is touch drag started");
                _dragOrigin = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }

            var ray = Camera.main.ScreenPointToRay(_dragOrigin);

            //Debug.Log("Ray: " + ray);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, Single.MaxValue, Gamefield.ChuzzleMask);
            if (hit.transform != null)
            {
                //Debug.Log("hit: " + hit.transform.gameObject);
                var wasNull = CurrentChuzzle == null;
                CurrentChuzzle = hit.transform.gameObject.transform.parent.GetComponent<Chuzzle>();
                if (wasNull)
                { 
                    _minY = _minX = float.MinValue;
                    _maxX = _maxY = float.MaxValue;
                }
            }

            return;
        }

        // CHECK DRAG STATE (Mouse or Touch)
        if ((!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) && 0 == Input.touchCount)
        {
            DropDrag();
            return;
        }

        if (CurrentChuzzle == null)
        {
            return;
        }


        if (Input.GetMouseButton(0)) // Get Position Difference between Last and Current Touches
        {
            // MOUSE
            _delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(_dragOrigin);
        }
        else
        {
            if (Input.touchCount > 0)
            {
                // TOUCH
                _deltaTouch =
                    Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x,
                        Input.GetTouch(0).position.y, 0));
                _delta = _deltaTouch - Camera.main.ScreenToWorldPoint(_dragOrigin);
            }
        }

        //Debug.Log("Delta: " + _delta);
        _delta = Vector3.ClampMagnitude(_delta, 0.45f*Chuzzle.Scale.x);

        if (!_axisChozen)
        {
            //chooze drag direction
            if (Mathf.Abs(_delta.x) < 1.5*Mathf.Abs(_delta.y) || Mathf.Abs(_delta.x) > 1.5*Mathf.Abs(_delta.y))
            {
                if (Mathf.Abs(_delta.x) < Mathf.Abs(_delta.y))
                {
                    SelectedChuzzles = draggableChuzzles.Where(x => x.Current.x == CurrentChuzzle.Current.x).ToList();
                    _isVerticalDrag = true;
                }
                else
                {
                    SelectedChuzzles = draggableChuzzles.Where(x => x.Current.y == CurrentChuzzle.Current.y).ToList();
                    _isVerticalDrag = false;
                }

                _hasLockedChuzzles = HasLockChuzzles;
                if (_hasLockedChuzzles)
                {
                    _minX = CurrentChuzzle.Current.Position.x - Chuzzle.Scale.x*0.4f;
                    _maxX = CurrentChuzzle.Current.Position.x + Chuzzle.Scale.x * 0.4f;
                    _minY = CurrentChuzzle.Current.Position.y - Chuzzle.Scale.y * 0.4f;
                    _maxY = CurrentChuzzle.Current.Position.y + Chuzzle.Scale.y * 0.4f;
                }

                _axisChozen = true;
                //Debug.Log("Direction chozen. Vertical: " + _isVerticalDrag);
            }
        }

        if (_axisChozen)
        {
            if (_isVerticalDrag)
            {
                CurrentDirection = _delta.y > 0 ? Direction.Top : Direction.Bottom;
                _delta.z = _delta.x = 0;
            }
            else
            {
                CurrentDirection = _delta.x > 0 ? Direction.Right : Direction.Left;
                _delta.y = _delta.z = 0;
            }
        }

        // RESET START POINT
        _dragOrigin = Input.mousePosition;

        #endregion
    }

    private void RepaintRandom()
    {
        Debug.Log("Random repaint");
        var possibleChuzzles = Gamefield.Level.ActiveChuzzles.Where(GamefieldUtility.IsUsual).ToArray();
        var randomChuzzle = possibleChuzzles[Random.Range(0, possibleChuzzles.Length)];

        TilesFactory.Instance.CreateChuzzle(randomChuzzle.Current);
        randomChuzzle.Destroy(false, false);
    }

    public void LateUpdateState(List<Cell> activeCells)
    {
        if (_isReturning)
        {
            foreach (var selectedChuzzle in SelectedChuzzles)
            {
                selectedChuzzle.transform.position += selectedChuzzle.Velocity*Time.deltaTime;
            }

            if (_isVerticalDrag)
            {
                CurrentDirection = CurrentChuzzle.Velocity.y > 0 ? Direction.Top : Direction.Bottom;
            }
            else
            {
                CurrentDirection = CurrentChuzzle.Velocity.x > 0 ? Direction.Right : Direction.Left;
            }

            MoveChuzzles(activeCells);

            var isAllOnPosition = true;
            foreach (var selectedChuzzle in SelectedChuzzles)
            {
                if (Vector3.Distance(selectedChuzzle.transform.position, selectedChuzzle.MoveTo.Position) < 0.1f)
                {
                    selectedChuzzle.Velocity = Vector3.zero;
                    selectedChuzzle.transform.position = selectedChuzzle.MoveTo.Position;
                }
                else
                {
                    isAllOnPosition = false;
                }
            }
            if (isAllOnPosition)
            {
                Reset();
            }
            return;
        }

        if (SelectedChuzzles.Any() && _axisChozen && _delta.magnitude >= 0.01f)
        {
            var pos = CurrentChuzzle.transform.position;

            //clamp drag
            if (_isVerticalDrag)
            {
                if (CurrentDirection == Direction.Top && Math.Abs(pos.y - _maxY) < 0.01f)
                {
                    return;
                }

                if (CurrentDirection == Direction.Bottom && Math.Abs(pos.y - _minY) < 0.01f)
                {
                    return;
                }


                var maybePosition = CurrentChuzzle.transform.position.y + _delta.y;
                var clampPosition = Mathf.Clamp(maybePosition, _minY, _maxY);

                if (Math.Abs(maybePosition - clampPosition) > 0.001f)
                {
                    _delta.y = clampPosition - maybePosition;
                }
            }
            else
            {
                if (CurrentDirection == Direction.Right && Math.Abs(pos.x - _maxX) < 0.01f)
                {
                    return;
                }

                if (CurrentDirection == Direction.Left && Math.Abs(pos.x - _minX) < 0.01f)
                {
                    return;
                }

                var maybePosition = CurrentChuzzle.transform.position.x + _delta.x;
                var clampPosition = Mathf.Clamp(maybePosition, _minX, _maxX);

                if (Math.Abs(maybePosition - clampPosition) > 0.001f)
                {
                    _delta.x = clampPosition - maybePosition;
                }
            }

            foreach (var c in SelectedChuzzles)
            {
                c.transform.position += _delta;
            }

            MoveChuzzles(activeCells);
        }
    }

    private void MoveChuzzles(List<Cell> activeCells)
    {
        foreach (var c in SelectedChuzzles)
        {
            var copyPosition = c.transform.position;

            var real = GamefieldUtility.ToRealCoordinates(c);
            var targetCell = GamefieldUtility.CellAt(activeCells, real.x, real.y);

            var difference = c.transform.position - GamefieldUtility.ConvertXYToPosition(real.x, real.y, Chuzzle.Scale); 

            var isNeedCopy = false;

            if (targetCell != null && !targetCell.IsTemporary)
            {
                if (!_isVerticalDrag)
                {
                    if (difference.x > 0)
                    {
                        isNeedCopy = targetCell.Right == null ||
                                     (targetCell.Right != null && targetCell.Right.Type != CellTypes.Usual);
                        if (isNeedCopy)
                        {
                            var rightCell = GetRightCell(activeCells, targetCell.Right, c);
                            copyPosition = rightCell.Position + difference - new Vector3(Chuzzle.Scale.x, 0, 0);
                        }
                    }
                    else
                    {
                        isNeedCopy = targetCell.Left == null ||
                                     (targetCell.Left != null && targetCell.Left.Type != CellTypes.Usual);
                        if (isNeedCopy)
                        {
                            var leftCell = GetLeftCell(activeCells, targetCell.Left, c);
                            copyPosition = leftCell.Position + difference + new Vector3(Chuzzle.Scale.x, 0, 0);
                        }
                    }
                }
                else
                {
                    if (difference.y > 0)
                    {
                        isNeedCopy = targetCell.Top == null ||
                                     (targetCell.Top != null &&
                                      (targetCell.Top.Type == CellTypes.Block || targetCell.Top.IsTemporary));
                        if (isNeedCopy)
                        {
                            var topCell = GetTopCell(activeCells, targetCell.Top, c);
                            copyPosition = topCell.Position + difference - new Vector3(0, Chuzzle.Scale.y, 0);
                        }
                    }
                    else
                    {
                        isNeedCopy = targetCell.Bottom == null ||
                                     (targetCell.Bottom != null && targetCell.Bottom.Type == CellTypes.Block);
                        if (isNeedCopy)
                        {
                            var bottomCell = GetBottomCell(activeCells, targetCell.Bottom, c);
                            copyPosition = bottomCell.Position + difference + new Vector3(0, Chuzzle.Scale.y, 0);
                        }
                    }
                }
            }
            else
            {
                isNeedCopy = true;
            }

            if (targetCell == null || targetCell.Type == CellTypes.Block || targetCell.IsTemporary)
            {
                switch (CurrentDirection)
                {
                    case Direction.Left:
                        //if border
                        targetCell = GetLeftCell(activeCells, targetCell, c);
                        break;
                    case Direction.Right:
                        targetCell = GetRightCell(activeCells, targetCell, c);
                        break;
                    case Direction.Top:
                        //if border
                        targetCell = GetTopCell(activeCells, targetCell, c);
                        break;
                    case Direction.Bottom:
                        targetCell = GetBottomCell(activeCells, targetCell, c);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Current direction can not be shit");
                }

                c.transform.position = targetCell.Position + difference;

                // Debug.Log("New coord: "+GamefieldUtility.ToRealCoordinates(c)+" for "+c.gameObject.name + " pos: "+c.transform.position);
            }

            if (difference.magnitude < (Chuzzle.Scale.x/25))
            {
                isNeedCopy = false;
            }

            if (isNeedCopy)
            {
                c.Teleportable.Show();
                c.Teleportable.Copy.transform.position = copyPosition;
            }
            else
            {
                c.Teleportable.Hide();
            }
        }
    }
   
    #region Get Cells
    private static Cell GetBottomCell(List<Cell> activeCells, Cell targetCell, Chuzzle c)
    {
//if border
        if (targetCell == null)
        {
            targetCell = GamefieldUtility.CellAt(activeCells, c.Current.x,
                activeCells.Where(x => !x.IsTemporary).Max(x => x.y));
            if (targetCell.Type == CellTypes.Block)
            {
                targetCell = targetCell.GetBottomWithType();
            }
        }
        else
        {
            targetCell = targetCell.GetBottomWithType();

            if (targetCell == null)
            {
                targetCell = GamefieldUtility.CellAt(activeCells, c.Current.x,
                    activeCells.Where(x => !x.IsTemporary).Max(x => x.y));
                if (targetCell.Type == CellTypes.Block)
                {
                    targetCell = targetCell.GetBottomWithType();
                }
            }
        }
        return targetCell;
    }

    private static Cell GetTopCell(List<Cell> activeCells, Cell targetCell, Chuzzle c)
    {
        if (targetCell == null || targetCell.IsTemporary)
        {
            targetCell = GamefieldUtility.CellAt(activeCells, c.Current.x,
                activeCells.Where(x => !x.IsTemporary).Min(x => x.y));
            if (targetCell.Type == CellTypes.Block)
            {
                targetCell = targetCell.GetTopWithType();
            }
        }
        else
        {
            targetCell = targetCell.GetTopWithType();

            if (targetCell == null)
            {
                targetCell = GamefieldUtility.CellAt(activeCells, c.Current.x,
                    activeCells.Where(x => !x.IsTemporary).Min(x => x.y));
                if (targetCell.Type == CellTypes.Block)
                {
                    targetCell = targetCell.GetTopWithType();
                }
            }
        }
        return targetCell;
    }

    private static Cell GetRightCell(List<Cell> activeCells, Cell targetCell, Chuzzle c)
    {
//if border
        if (targetCell == null)
        {
            targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Min(x => x.x), c.Current.y);
            if (targetCell.Type == CellTypes.Block)
            {
                targetCell = targetCell.GetRightWithType();
            }
        }
        else
        {
            targetCell = targetCell.GetRightWithType();
            if (targetCell == null)
            {
                targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Min(x => x.x),
                    c.Current.y);
                if (targetCell.Type == CellTypes.Block)
                {
                    targetCell = targetCell.GetRightWithType();
                }
            }
        }
        return targetCell;
    }

    private static Cell GetLeftCell(List<Cell> activeCells, Cell targetCell, Chuzzle c)
    {
        if (targetCell == null)
        {
            targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Max(x => x.x), c.Current.y);
            if (targetCell.Type == CellTypes.Block)
            {
                targetCell = targetCell.GetLeftWithType();
            }
        }
        else
        {
            //if block
            targetCell = targetCell.GetLeftWithType();

            if (targetCell == null)
            {
                targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Max(x => x.x),
                    c.Current.y);
                if (targetCell.Type == CellTypes.Block)
                {
                    targetCell = targetCell.GetLeftWithType();
                }
            }
        }
        return targetCell;
    }
    #endregion
    
    private void DropDrag()
    {
        if (SelectedChuzzles.Any())
        {
            //drop shining
            foreach (var chuzzle in Gamefield.Level.ActiveChuzzles)
            {
                chuzzle.Shine = false;
                chuzzle.Teleportable.Hide();
            }

            //move all tiles to new real coordinates
            foreach (var chuzzle in SelectedChuzzles)
            {
                chuzzle.Real = Gamefield.Level.GetCellAt(GamefieldUtility.ToRealCoordinates(chuzzle),false);
            }

            foreach (var c in Gamefield.Level.Chuzzles)
            {
                c.MoveTo = c.Real;
            }

            foreach (var chuzzle in Gamefield.Level.Chuzzles)
            {   
                chuzzle.AnimateMoveTo(chuzzle.MoveTo.Position, 0.1f);
            }

            CheckAnimationCompleted();
        }
    }

    

    public void Reset()
    {
        foreach (var selectedChuzzle in SelectedChuzzles)
        {
            selectedChuzzle.Teleportable.Hide();
        }
        SelectedChuzzles.Clear();
        CurrentChuzzle = null;
        _axisChozen = false;
        _isVerticalDrag = false;
        _isReturning = false;
    }

    public override void UpdateState()
    {
        if (!AnimatedChuzzles.Any())
        {
            UpdateState(Gamefield.Level.ActiveChuzzles);
        }
    }

    public void CheckAnimationCompleted()
    {
        if (!AnimatedChuzzles.Any()) 
        {
            CheckCombinations();
        }
    }


    public override void LateUpdateState()
    {
        if (!AnimatedChuzzles.Any())
        {
            LateUpdateState(Gamefield.Level.ActiveCells);
        }
    }
    
    public bool HasLockChuzzles
    {
        get
        {
            return SelectedChuzzles.Any(c => c is LockChuzzle);
        }
    }
}