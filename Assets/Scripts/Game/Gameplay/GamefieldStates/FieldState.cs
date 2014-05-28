#region

using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

#endregion

namespace GamefieldStates
{
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
            TilesCollection = Gamefield.Chuzzles.GetTiles();
            TilesCollection.AnimationFinished += OnAnimationFinished;
            //  if (!Tutorial.isActive)
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
            var numberOfTries = 0;
            do
            {
                if (GamefieldUtility.Repaint(numberOfTries)) break;

                _possibleCombination = GamefieldUtility.Tip(Gamefield.Level.Chuzzles.GetTiles(), out _targetPosition,
                    out _arrowChuzzle);
                Debug.Log(string.Format("Tip. From: {0} To: {1}", _arrowChuzzle, _targetPosition));
                numberOfTries++;
            } 
            while (!_possibleCombination.Any());
        }

        public override void OnExit()
        {
            if (TilesCollection.IsAnyAnimated)
            {
                Debug.LogError("FUCK you in field state: " + TilesCollection.Count);
            }
            Gamefield.GameMode.HumanTurn();
            Reset();
        }

        public void CheckCombinations()
        {
            var combinations = GamefieldUtility.FindCombinations(Gamefield.Chuzzles);
            if (combinations.Any() /*&& (!Tutorial.isActive || (Tutorial.isActive && CurrentChuzzle!=null&& Tutorial.Instance.IsTargetCell(CurrentChuzzle.Real)))*/)
            {
                foreach (var c in Gamefield.Chuzzles)
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
                size = Gamefield.Level.Cells.Height*Chuzzle.Scale.y;
            }
            else
            {
                size = Gamefield.Level.Cells.Width*Chuzzle.Scale.x;
            }
        
            var difference = CurrentChuzzle.Real.Position - CurrentChuzzle.Current.Position;
            var velocity = -returnSpeed*difference.normalized;
            /*
        Debug.Log("differnce: "+difference);
        Debug.Log("size: "+size);
        Debug.Log("start: "+CurrentChuzzle.Real.Position);
        Debug.Log("end: "+CurrentChuzzle.Current.Position);
        */
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

        private void OnAnimationFinished()
        {
            CheckAnimationCompleted();
        }

        #endregion

        public void UpdateState(IEnumerable<Chuzzle> draggableChuzzles)
        {
            if (_isReturning)
            {
                return;
            }
        
            /*
        TimeFromTip += Time.deltaTime;
        if (TimeFromTip > 1)
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
        */
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

            /*
        if (CurrentChuzzle && !Tutorial.CanTakeOnlyThisChuzzle(CurrentChuzzle))
        {
            Reset();
            return;
        }
        */

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

        public void LateUpdateState(IEnumerable<Cell> activeCells)
        {
            //tipArrow.UpdateState();
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

        private void MoveChuzzles(IEnumerable<Cell> activeCells)
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
        private static Cell GetBottomCell(IEnumerable<Cell> activeCells, Cell targetCell, Chuzzle c)
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

        private static Cell GetTopCell(IEnumerable<Cell> activeCells, Cell targetCell, Chuzzle c)
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

        private static Cell GetRightCell(IEnumerable<Cell> activeCells, Cell targetCell, Chuzzle c)
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

        private static Cell GetLeftCell(IEnumerable<Cell> activeCells, Cell targetCell, Chuzzle c)
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
                foreach (var chuzzle in Gamefield.Chuzzles)
                {
                    chuzzle.Shine = false;
                    chuzzle.Teleportable.Hide();
                }

                //move all tiles to new real coordinates
                foreach (var chuzzle in SelectedChuzzles)
                {
                    chuzzle.Real = Gamefield.Level.Cells.GetCellAt(GamefieldUtility.ToRealCoordinates(chuzzle),false);
                }

                foreach (var c in Gamefield.Chuzzles)
                {
                    c.MoveTo = c.Real;
                }

                foreach (var chuzzle in Gamefield.Chuzzles)
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
            if (!TilesCollection.Any())
            {
                UpdateState(Gamefield.Chuzzles);
            }
        }

        public void CheckAnimationCompleted()
        {
            if (!TilesCollection.Any()) 
            {
                CheckCombinations();
            }
        }


        public override void LateUpdateState()
        {
            if (!TilesCollection.Any())
            {
                LateUpdateState(Gamefield.Level.Cells.GetCells());
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
}