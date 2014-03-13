#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
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
    public TipArrow tipArrow;
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
    private List<Chuzzle> _possibleCombination;

    IntVector2 _targetPosition = null;
    Chuzzle _arrowChuzzle = null;
    public float returnSpeed = 3f;

    #region Event Handlers

    public override void OnEnter()
    {
        AnimatedChuzzles.Clear();
        Chuzzle.DropEventHandlers();
        Chuzzle.AnimationStarted += OnAnimationStarted;
        if (!Tutorial.isActive)
        {
            CheckPossibleCombinations();
        }


        if (!Gamefield.InvaderWasDestroyed)
        {
            InvaderChuzzle.Populate(Gamefield);
        }
        Gamefield.InvaderWasDestroyed = false;
    }

    private void CheckPossibleCombinations()
    {
        _targetPosition = null;
        _arrowChuzzle = null;
        _possibleCombination = GamefieldUtility.Tip(Gamefield.Level.ActiveChuzzles, out _targetPosition,
            out _arrowChuzzle);

        var numberOfTries = 0;
        while (!_possibleCombination.Any())
        {   
            numberOfTries++;

            var possible = Gamefield.Chuzzles.Where(GamefieldUtility.IsUsual);
             //complex logic of repainting
                                                 
             //check number of invaders
             //if more then third of maximum and possible less then 10
            if (InvaderChuzzle.AllInvaderChuzzles.Count > InvaderChuzzle.MaxInvadersOnLevel/3 && possible.Count() < 10)
            {
                //Debug.Log("Repaint invaders");
                var invadersForRepaint =
                    InvaderChuzzle.AllInvaderChuzzles.Where(
                        x =>
                            InvaderChuzzle.AllInvaderChuzzles.IndexOf(x) <
                            InvaderChuzzle.MaxInvadersOnLevel/3).ToArray();
                //repaint them to random color
                foreach (var invaderToReplace in invadersForRepaint)
                {
                    TilesFactory.Instance.ReplaceWithRandom(invaderToReplace);
                }
                //TODO show message to player                               
            }
            else
            {
                //try to find pair and repaint only one
                var combinations = GamefieldUtility.FindCombinations(Gamefield.Chuzzles, 2);
                if (combinations.Any())
                {
                    foreach (var comb in combinations)
                    {
                        //if vertical 
                        if (comb[0].Current.x == comb[1].Current.x)
                        {
                            //try to find up and bottom
                            var top = comb[0].Current.y > comb[1].Current.y ? comb[0] : comb[1];
                            var bottom = top == comb[0] ? comb[1] : comb[0];

                            var repainted = false;
                            if (top.Current.Top != null && top.Current.Top.Type != CellTypes.Block)
                            {
                                var possibleAbove =
                                    Gamefield.Chuzzles.Where(
                                        x =>
                                            GamefieldUtility.IsUsual(x) && x.Current.y == top.Current.Top.y &&
                                            x.Current != top.Current.Top)
                                        .ToArray();

                                if (possibleAbove.Any())
                                {
                                    Chuzzle toReplace = possibleAbove[Random.Range(0, possibleAbove.Length)];
                                    TilesFactory.Instance.ReplaceWithColor(toReplace, top.Color);
                                    //    Debug.Log("Repaint above pair");
                                    repainted = true;
                                }
                            }

                            if (bottom.Current.Bottom != null &&
                                bottom.Current.Bottom.Type != CellTypes.Block && !repainted)
                            {
                                var possibleBellow =
                                    Gamefield.Chuzzles.Where(
                                        x =>
                                            GamefieldUtility.IsUsual(x) &&
                                            x.Current.y == bottom.Current.Bottom.y &&
                                            x.Current != bottom.Current.Bottom)
                                        .ToArray();

                                if (possibleBellow.Any())
                                {
                                    Chuzzle toReplace =
                                        possibleBellow[Random.Range(0, possibleBellow.Length)];
                                    TilesFactory.Instance.ReplaceWithColor(
                                        toReplace, bottom.Color);
                                    repainted = true;
                                    //      Debug.Log("Repaint bellow pair");
                                }
                            }

                            if (repainted)
                            {
                                break;
                            }
                        }
                        else
                        {
                            //horizontal pair

                            var left = comb[0].Current.x < comb[1].Current.x ? comb[0] : comb[1];
                            var right = comb[0] == left ? comb[1] : comb[0];

                            var repainted = false;

                            //left?
                            if (left.Current.Left != null && left.Current.Left.Type != CellTypes.Block)
                            {
                                var possibleLeft =
                                    Gamefield.Chuzzles.Where(
                                        x =>
                                            GamefieldUtility.IsUsual(x) &&
                                            x.Current.x == left.Current.Left.x &&
                                            x.Current != left.Current.Left)
                                        .ToArray();

                                if (possibleLeft.Any())
                                {
                                    Chuzzle toReplace = possibleLeft[Random.Range(0, possibleLeft.Length)];
                                    TilesFactory.Instance.ReplaceWithColor(toReplace, left.Color);
                                    //        Debug.Log("Repaint left pair");
                                    repainted = true;
                                }
                            }

                            //right?
                            if (right.Current.Right != null && right.Current.Right.Type != CellTypes.Block &&
                                !repainted)
                            {
                                var possibleRight =
                                    Gamefield.Chuzzles.Where(
                                        x =>
                                            GamefieldUtility.IsUsual(x) &&
                                            x.Current.x == right.Current.Right.x &&
                                            x.Current != right.Current.Right)
                                        .ToArray();

                                if (possibleRight.Any())
                                {
                                    Chuzzle toReplace = possibleRight[Random.Range(0, possibleRight.Length)];
                                    TilesFactory.Instance.ReplaceWithColor(
                                        toReplace, right.Color);
                                    repainted = true;
                                    //          Debug.Log("Repaint right pair");
                                }
                            }

                            if (repainted)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //if now pairs
                    if (numberOfTries > 5)
                    {
                        //create combination guaranteed
                        //get random usual cell
                        var possibleCellsLeftLeft = Gamefield.Level.Cells.Where(
                            cell =>
                                //слева есть обычная клетка и слева слева тоже есть обычная клетка и в стобце левее левого есть еще одна обычная клетка и в ней находится обычный тайл
                                (cell.Left != null && cell.Left.Type == CellTypes.Usual &&
                                 (cell.Left.Left != null && cell.Left.Left.Type == CellTypes.Usual &&
                                  Gamefield.Level.Cells.Count(x => x.x == cell.Left.Left.x && x.Type == CellTypes.Usual) >
                                  1) && Gamefield.Chuzzles.FirstOrDefault(chuzzle => chuzzle.Current == cell) != null && GamefieldUtility.IsUsual(Gamefield.Chuzzles.FirstOrDefault(chuzzle => chuzzle.Current == cell)))
                            ).ToList();
                        if (possibleCellsLeftLeft.Any())
                        {
                            var randomLeftLeft = possibleCellsLeftLeft[Random.Range(0, possibleCellsLeftLeft.Count)];
                            var randomLeftLeftChuzzle = GamefieldUtility.GetChuzzleInCell(randomLeftLeft,
                                Gamefield.Chuzzles);
                            TilesFactory.Instance.ReplaceWithColor(
                                GamefieldUtility.GetChuzzleInCell(randomLeftLeft.Left, Gamefield.Chuzzles),
                                randomLeftLeftChuzzle.Color);
                            var possibleLeftLeftLeft =
                                Gamefield.Chuzzles.Where(
                                    x =>
                                        x.Current != randomLeftLeft.Left.Left &&
                                        x.Current.x == randomLeftLeft.Left.Left.x &&
                                        (GamefieldUtility.IsUsual(x) || !GamefieldUtility.IsOrdinaryDestroyable(x)))
                                    .ToList();
                            TilesFactory.Instance.ReplaceWithColor(
                                possibleLeftLeftLeft[Random.Range(0, possibleLeftLeftLeft.Count)],
                                randomLeftLeftChuzzle.Color);
                            Debug.Log("Random left left");
                        }
                        else
                        {
                            Debug.LogWarning("All our life is a lie");
                            break;
                        }
                        //check if has free neighbour
                        //repaint to same color
                        //repaint random tile in near row to same color
                    }
                    else
                    {
                        var possibleChuzzles = Gamefield.Chuzzles.Where(GamefieldUtility.IsUsual).ToArray();

                        for (int index = 0; index < possibleChuzzles.Length; index++)
                        {
                            var possibleChuzzle = possibleChuzzles[index];
                            TilesFactory.Instance.ReplaceWithRandom(possibleChuzzle);
                        }
                    }
                }
            }


            //if create combination - repaint random
            var combination = GamefieldUtility.FindOnlyOneCombination(Gamefield.Chuzzles);
            if (combination.Any())
            {
                foreach (var chuzzle in combination)
                {
                    if (GamefieldUtility.IsUsual(chuzzle))
                    {
                        //Debug.Log("Oops, combination. Repaint");
                        TilesFactory.Instance.ReplaceWithOtherColor(chuzzle);
                        break;
                    }
                }
            }
            
            
            _possibleCombination = GamefieldUtility.Tip(Gamefield.Level.ActiveChuzzles, out _targetPosition,
                out _arrowChuzzle);
            //Debug.Log("Arrow chuzzle: " + _arrowChuzzle);
            //Debug.Log("Target position: " + _targetPosition);
        }
        //Debug.Log("Repainted in "+numberOfTries + " attempts");
    }

    public override void OnExit()
    {
        if (AnimatedChuzzles.Any())
        {
            Debug.LogError("FUCK you in field state: " + AnimatedChuzzles.Count);
        }
        Gamefield.GameMode.HumanTurn();
        Reset();
    }

    public void CheckCombinations()
    {
        var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
        if (combinations.Any() && (!Tutorial.isActive || (Tutorial.isActive && CurrentChuzzle!=null&& Tutorial.instance.IsTargetCell(CurrentChuzzle.Real))))
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
                StartReturn();
            }
        }
    }

    private void StartReturn()
    {
        var size = 0f;
        if (_isVerticalDrag)
        {
            size = Gamefield.Level.Height*Chuzzle.Scale.y;
        }
        else
        {
            size = Gamefield.Level.Width*Chuzzle.Scale.x;
        }
        
        var difference = CurrentChuzzle.Real.Position - CurrentChuzzle.Current.Position;
        var velocity = -returnSpeed*difference.normalized;

        Debug.Log("differnce: "+difference);
        Debug.Log("size: "+size);
        Debug.Log("start: "+CurrentChuzzle.Real.Position);
        Debug.Log("end: "+CurrentChuzzle.Current.Position);

        if (difference.magnitude > size - difference.magnitude)
        {
            velocity *= -1;
        }

        foreach (var c in SelectedChuzzles)
        {
            c.MoveTo = c.Real = c.Current;
            c.Velocity = velocity;
        }

        if (_isVerticalDrag)
        {
            CurrentDirection = CurrentChuzzle.Velocity.y > 0 ? Direction.Top : Direction.Bottom;            
        }
        else
        {
            CurrentDirection = CurrentChuzzle.Velocity.x > 0 ? Direction.Right : Direction.Left;
        }

        _isReturning = true;
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
            if (_possibleCombination.Any() && _arrowChuzzle)
            {
                foreach (var chuzzle in _possibleCombination)
                {
                    chuzzle.Shine = true;
                }
                GamefieldUtility.ShowArrow(_arrowChuzzle, _targetPosition, tipArrow);
            }

            TimeFromTip = 0;
        }       

        #region Drag

        if (CurrentChuzzle == null && (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            _dragOrigin = Input.mousePosition;
           // Debug.Log("Position: " + _dragOrigin);

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //   Debug.Log("is touch drag started");
                _dragOrigin = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }

            var ray = Camera.main.ScreenPointToRay(_dragOrigin);

        //    Debug.Log("Ray: " + ray);
            Debug.DrawRay(ray.origin, ray.direction*Single.MaxValue);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, Single.MaxValue, Gamefield.ChuzzleMask);
            if (hit.transform != null)
            {
              //  Debug.Log("hit: " + hit.transform.gameObject);
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

        if (CurrentChuzzle && Tutorial.isActive && !Tutorial.instance.CanTakeOnlyThisChuzzle(CurrentChuzzle))
        {
            Reset();
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

    public void LateUpdateState(List<Cell> activeCells)
    {
        tipArrow.UpdateState();
        if (_isReturning)
        {
            foreach (var selectedChuzzle in SelectedChuzzles)
            {
                var change = selectedChuzzle.Velocity*Time.deltaTime;
                var currentPos = selectedChuzzle.transform.position;
                var nextPos = currentPos + change;
                var moveToPos = selectedChuzzle.MoveTo.Position;
                switch (CurrentDirection)
                {
                    case Direction.Right:
                        if (currentPos.x < moveToPos.x && nextPos.x > moveToPos.x)
                        {
                            selectedChuzzle.transform.position = selectedChuzzle.MoveTo.Position;
                            selectedChuzzle.Velocity = Vector3.zero;
                            continue;
                        }
                        break;
                    case Direction.Left:
                        if (currentPos.x > moveToPos.x && nextPos.x < moveToPos.x)
                        {
                            selectedChuzzle.transform.position = selectedChuzzle.MoveTo.Position;
                            selectedChuzzle.Velocity = Vector3.zero;
                            continue;
                        }
                        break;
                    case Direction.Top:
                        if (currentPos.y < moveToPos.y && nextPos.y > moveToPos.y)
                        {
                            selectedChuzzle.transform.position = selectedChuzzle.MoveTo.Position;
                            selectedChuzzle.Velocity = Vector3.zero;
                            continue;
                        }
                        break;
                    case Direction.Bottom:
                        if (currentPos.y > moveToPos.y && nextPos.y < moveToPos.y)
                        {
                            selectedChuzzle.transform.position = selectedChuzzle.MoveTo.Position;
                            selectedChuzzle.Velocity = Vector3.zero;
                            continue;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                selectedChuzzle.transform.position = nextPos;
            }

            MoveChuzzles(activeCells);
            
            if (SelectedChuzzles.All(x=>x.Velocity == Vector3.zero))
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

    private void RevertVelocity()
    {
        foreach (var selectedChuzzle in SelectedChuzzles)
        {
            selectedChuzzle.Velocity *= -0.9f;
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