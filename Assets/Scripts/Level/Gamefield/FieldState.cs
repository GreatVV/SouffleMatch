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
    public float TimeFromTip = 0;
    public GameObject DownArrow;

    #region Direction enum

    public enum Direction
    {
        ToLeft,
        ToRight,
        ToTop,
        ToBottom
    };

    #endregion

    private Vector3 _delta;
    private Vector3 _deltaTouch;
    private bool _directionChozen;
    private Vector3 _dragOrigin;
    private Chuzzle _draggable;
    private bool _isVerticalDrag;
    public List<Chuzzle> SelectedChuzzles = new List<Chuzzle>();
    public Chuzzle CurrentChuzzle;
    public Direction CurrentDirection;

    // Update is called once per frame
    public void UpdateState(IEnumerable<Chuzzle> draggableChuzzles)
    {
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

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            _dragOrigin = Input.mousePosition;
            Debug.Log("Position: " + _dragOrigin);

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //   Debug.Log("is touch drag started");
                _dragOrigin = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }

            var ray = Camera.main.ScreenPointToRay(_dragOrigin);

            Debug.Log("Ray: " + ray);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, Single.MaxValue, Gamefield.ChuzzleMask);
            if (hit.transform != null)
            {
                Debug.Log("hit: " + hit.transform.gameObject);
                CurrentChuzzle = hit.transform.gameObject.transform.parent.GetComponent<Chuzzle>();
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

        Debug.Log("Delta: " + _delta);
        _delta = Vector3.ClampMagnitude(_delta, CurrentChuzzle.Scale.x);


        if (!_directionChozen)
        {
            //chooze drag direction
            if (Mathf.Abs(_delta.x) < 1.5*Mathf.Abs(_delta.y) || Mathf.Abs(_delta.x) > 1.5*Mathf.Abs(_delta.y))
            {
                if (Mathf.Abs(_delta.x) < Mathf.Abs(_delta.y))
                {
                    //TODO: choose row
                    SelectedChuzzles = draggableChuzzles.Where(x => x.Current.x == CurrentChuzzle.Current.x).ToList();
                    _isVerticalDrag = true;
                }
                else
                {
                    //TODO: choose column
                    SelectedChuzzles = draggableChuzzles.Where(x => x.Current.y == CurrentChuzzle.Current.y).ToList();
                    _isVerticalDrag = false;
                }

                _directionChozen = true;
                //Debug.Log("Direction chozen. Vertical: " + _isVerticalDrag);
            }
        }

        if (_directionChozen)
        {
            if (_isVerticalDrag)
            {
                CurrentDirection = _delta.y > 0 ? Direction.ToTop : Direction.ToBottom;
            }
            else
            {
                CurrentDirection = _delta.x > 0 ? Direction.ToLeft : Direction.ToRight;
            }
        }

        // RESET START POINT
        _dragOrigin = Input.mousePosition;

        #endregion
    }

    private void RepaintRandom()
    {
        Debug.Log("Random repaint");
        var randomChuzzle = Gamefield.Level.Chuzzles[Random.Range(0, Gamefield.Level.Chuzzles.Count)];

        Gamefield.Level.CreateRandomChuzzle(randomChuzzle.Current.x, randomChuzzle.Current.y, true);

        Object.Destroy(randomChuzzle.gameObject);
        Gamefield.Level.ActiveChuzzles.Remove(randomChuzzle);
        Gamefield.Level.Chuzzles.Remove(randomChuzzle);
    }

    public void LateUpdateState(List<Cell> activeCells)
    {
        if (SelectedChuzzles.Any() && _directionChozen)
        {
            foreach (var c in SelectedChuzzles)
            {
                c.transform.localPosition += _isVerticalDrag ? new Vector3(0, _delta.y, 0) : new Vector3(_delta.x, 0, 0);



                var copyPosition = c.transform.position;
                var real = GamefieldUtility.ToRealCoordinates(c);
                var targetCell = GamefieldUtility.CellAt(activeCells, real.x, real.y);

                var difference = c.transform.localPosition -
                                     GamefieldUtility.ConvertXYToPosition(real.x, real.y, c.Scale);

                bool isNeedCopy = false;

                if (targetCell != null && !targetCell.IsTemporary)
                {
                    switch (CurrentDirection)
                    {
                        case Direction.ToLeft:
                        case Direction.ToRight:

                            if (c.transform.position.x > targetCell.Sprite.transform.position.x)
                            {
                                isNeedCopy = targetCell.Right == null || (targetCell.Right != null && targetCell.Right.Type != CellTypes.Usual);
                                if (isNeedCopy)
                                {
                                    var rightCell = GetRightCell(activeCells, targetCell.Right, c);
                                    copyPosition = GamefieldUtility.ConvertXYToPosition(rightCell.x, rightCell.y, c.Scale) + difference - new Vector3(c.Scale.x, 0, 0);
                                }
                            }
                            else
                            {
                                isNeedCopy = targetCell.Left == null || (targetCell.Left != null && targetCell.Left.Type != CellTypes.Usual);
                                if (isNeedCopy)
                                {
                                    var leftCell = GetLeftCell(activeCells, targetCell.Left, c);
                                    copyPosition = GamefieldUtility.ConvertXYToPosition(leftCell.x, leftCell.y, c.Scale) + difference + new Vector3(c.Scale.x, 0, 0);
                                }
                            }    
                            break;
                        case Direction.ToTop:
                        case Direction.ToBottom:
                            Debug.Log("TargetCell: "+targetCell);
                            Debug.Log("Sprite: "+targetCell.Sprite);
                            if (c.transform.position.y > targetCell.Sprite.transform.position.y)
                            {

                                isNeedCopy = targetCell.Top == null ||
                                             (targetCell.Top != null &&
                                              (targetCell.Top.Type == CellTypes.Block || targetCell.Top.IsTemporary));
                                if (isNeedCopy)
                                {
                                    var topCell = GetTopCell(activeCells, targetCell.Top, c);
                                    copyPosition = GamefieldUtility.ConvertXYToPosition(topCell.x, topCell.y, c.Scale) +
                                                   difference - new Vector3(0, c.Scale.y, 0);
                                }
                            }
                            else
                            {                
                                isNeedCopy = targetCell.Bottom == null ||
                                             (targetCell.Bottom != null && targetCell.Bottom.Type == CellTypes.Block);
                                if (isNeedCopy)
                                {
                                    var bottomCell = GetBottomCell(activeCells, targetCell.Bottom, c);
                                    copyPosition =
                                        GamefieldUtility.ConvertXYToPosition(bottomCell.x, bottomCell.y, c.Scale) +
                                        difference + new Vector3(0, c.Scale.y, 0);
                                }
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
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
                        case Direction.ToRight:
                            //if border
                            targetCell = GetLeftCell(activeCells, targetCell, c);    
                            break;
                        case Direction.ToLeft:
                            targetCell = GetRightCell(activeCells, targetCell, c);   
                            break;
                        case Direction.ToTop:
                            //if border
                            targetCell = GetTopCell(activeCells, targetCell, c);
                            break;
                        case Direction.ToBottom:
                            targetCell = GetBottomCell(activeCells, targetCell, c);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Current direction can not be shit");
                    }

                    c.transform.localPosition = GamefieldUtility.ConvertXYToPosition(targetCell.x, targetCell.y, c.Scale) + difference;
                }
              
                if (isNeedCopy)
                {
                    var teleportable = c.GetComponent<TeleportableEntity>();
                    if (teleportable != null)
                    {
                        if (!teleportable.HasCopy)
                        {
                            teleportable.CreateCopy();
                        }
                        Debug.Log("Copyied: "+teleportable);
                        teleportable.Copy.transform.position = copyPosition;
                    }
                }
            }
        }
    }

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

    private void DropDrag()
    {
        if (SelectedChuzzles.Any())
        {
            //drop shining
            foreach (var chuzzle in Gamefield.Level.ActiveChuzzles)
            {
                chuzzle.Shine = false;
            }

            //move all tiles to new real coordinates
            foreach (var c in SelectedChuzzles)
            {
                CalculateRealCoordinatesFor(c);
                c.GetComponent<TeleportableEntity>().DestroyCopy();
            }

            foreach (var c in Gamefield.Level.Chuzzles)
            {
                c.MoveTo = c.Real;
            }

            var anyMove = MoveToTargetPosition(Gamefield.Level.Chuzzles);
            if (!anyMove)
            {
                OnChuzzleCompletedTweens();
            }

            Reset();
        }
    }

    public void Reset()
    {
        SelectedChuzzles.Clear();
        CurrentChuzzle = null;
        _directionChozen = false;
        _isVerticalDrag = false;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void UpdateState()
    {
        if (!AnimatedChuzzles.Any())
        {
            UpdateState(Gamefield.Level.ActiveChuzzles);
        }
    }

    public override void LateUpdateState()
    {
        if (!AnimatedChuzzles.Any())
        {
            LateUpdateState(Gamefield.Level.ActiveCells);
        }
    }

    public List<Chuzzle> AnimatedChuzzles = new List<Chuzzle>();

    public void OnChuzzleCompletedTweens()
    {
        var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
        if (combinations.Any())
        {
            foreach (var c in Gamefield.Level.Chuzzles)
            {
                c.Current = c.MoveTo = c.Real;
            }
            Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);

            Gamefield.GameMode.HumanTurn();
        }
        else
        {
            foreach (var c in Gamefield.Level.Chuzzles)
            {
                c.MoveTo = c.Real = c.Current;
            }
            MoveToTargetPosition(Gamefield.Level.Chuzzles);
        }
    }

    public bool MoveToTargetPosition(List<Chuzzle> targetChuzzles)
    {
        var isAnyTween = false;
        foreach (var c in targetChuzzles)
        {
            var targetPosition = new Vector3(c.MoveTo.x*c.Scale.x, c.MoveTo.y*c.Scale.y, 0);
            if (Vector3.Distance(c.transform.localPosition, targetPosition) > 0.1f)
            {
                isAnyTween = true;
                AnimatedChuzzles.Add(c);
                iTween.MoveTo(c.gameObject,
                    iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f,
                        "oncomplete", new Action<object>(OnTweenMoveAfterDrag), "oncompletetarget", Gamefield.gameObject,
                        "oncompleteparams", c));
            }
            else
            {
                c.transform.localPosition = targetPosition;
            }
        }

        return isAnyTween;
    }

    public void CalculateRealCoordinatesFor(Chuzzle chuzzle)
    {
        chuzzle.Real = Gamefield.Level.GetCellAt(Mathf.RoundToInt(chuzzle.transform.localPosition.x/chuzzle.Scale.x),
            Mathf.RoundToInt(chuzzle.transform.localPosition.y/chuzzle.Scale.y), false);
    }

    private void OnTweenMoveAfterDrag(object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        if (AnimatedChuzzles.Contains(chuzzle))
        {
            AnimatedChuzzles.Remove(chuzzle);
        }

        if (!AnimatedChuzzles.Any())
        {
            OnChuzzleCompletedTweens();
        }
    }
}