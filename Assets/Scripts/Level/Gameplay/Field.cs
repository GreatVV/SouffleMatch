using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class Field : GamefieldState
{
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

   
    public Field(Gamefield gamefield) : base(gamefield)
    {
    }               
 

    // Update is called once per frame
    public void Update(IEnumerable<Chuzzle> draggableChuzzles)
    {
        Gamefield.TimeFromTip += Time.deltaTime;
        if (Gamefield.TimeFromTip > 1 && !Gamefield.Level.ActiveChuzzles.Any(x=>x.Shine))
        {
            IntVector2 targetPosition = null;
            Chuzzle arrowChuzzle = null;
            var possibleCombination = GamefieldUtility.Tip(Gamefield.Level.ActiveChuzzles,out targetPosition, out arrowChuzzle);
            if (possibleCombination.Any())
            {
                foreach (var chuzzle in possibleCombination)
                {
                    chuzzle.Shine = true;
                }
                GamefieldUtility.ShowArrow(arrowChuzzle, targetPosition, Gamefield.DownArrow);
            }
            else
            {  
                RepaintRandom();
                return;
            }

            
            Gamefield.TimeFromTip = 0;
        }

        #region Drag

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            _dragOrigin = Input.mousePosition;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //   Debug.Log("is touch drag started");
                _dragOrigin = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }


            var ray = Camera.main.ScreenPointToRay(_dragOrigin);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Single.MaxValue, Gamefield.ChuzzleMask))
            {
                CurrentChuzzle = hit.transform.gameObject.GetComponent<Chuzzle>();
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
            _delta = Input.mousePosition - _dragOrigin;

            //   Debug.Log("Drag delta");
        }
        else
        {
            if (Input.touchCount > 0)
            {
                // TOUCH
                _deltaTouch = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
                _delta = _deltaTouch - _dragOrigin;
                // Debug.Log("Drag delta TOUCH");
            }
        }

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

    public void LateUpdate(List<Cell> activeCells)
    {
        if (SelectedChuzzles.Any() && _directionChozen)
        {
            foreach (var c in SelectedChuzzles)
            {
                c.GetComponent<TeleportableEntity>().prevPosition = c.transform.localPosition;
                c.transform.localPosition += _isVerticalDrag ? new Vector3(0, _delta.y, 0) : new Vector3(_delta.x, 0, 0);

                var real = GamefieldUtility.ToRealCoordinates(c);
                var targetCell = GamefieldUtility.CellAt(activeCells, real.x, real.y);
                if (targetCell == null || targetCell.Type == CellTypes.Block || targetCell.IsTemporary)
                {
                    //TODO create a copy of sprite at current position

                    // Debug.Log("Teleport from " + currentCell);
                    switch (CurrentDirection)
                    {
                        case Field.Direction.ToRight:
                            //if border
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
                            break;
                        case Field.Direction.ToLeft:
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
                            break;
                        case Field.Direction.ToTop:
                            //if border
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
                            break;
                        case Field.Direction.ToBottom:
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
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Current direction can not be shit");
                    }
                    //  Debug.Log("Teleport to " + targetCell);

                    var difference = c.transform.localPosition -
                                     GamefieldUtility.ConvertXYToPosition(real.x, real.y, c.Scale);
                    c.transform.localPosition =
                        GamefieldUtility.ConvertXYToPosition(targetCell.x, targetCell.y, c.Scale) +
                        difference;
                }
            }
        }
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

    public override void Update()
    {
        if (!AnimatedChuzzles.Any())
        {
            Update(Gamefield.Level.ActiveChuzzles);
        }
    }

    public override void LateUpdate()
    {
        if (!AnimatedChuzzles.Any())
        {
            LateUpdate(Gamefield.Level.ActiveCells);
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
            Gamefield.SwitchStateTo(Gamefield.CheckSpecial);

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
            var targetPosition = new Vector3(c.MoveTo.x * c.Scale.x, c.MoveTo.y * c.Scale.y, 0);
            if (Vector3.Distance(c.transform.localPosition, targetPosition) > 0.1f)
            {
                isAnyTween = true;
                AnimatedChuzzles.Add(c);
                iTween.MoveTo(c.gameObject,
                    iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f,
                        "oncomplete", new Action<object>(OnTweenMoveAfterDrag), "oncompletetarget", Gamefield.gameObject, "oncompleteparams", c));
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
        chuzzle.Real = Gamefield.Level.GetCellAt(Mathf.RoundToInt(chuzzle.transform.localPosition.x / chuzzle.Scale.x),
            Mathf.RoundToInt(chuzzle.transform.localPosition.y / chuzzle.Scale.y), false);
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