#region

using System;
using System.Collections.Generic;
using System.Linq;
using Game.Gameplay.Cells;
using Game.Gameplay.Chuzzles;
using Game.Gameplay.Chuzzles.Types;
using Game.Gameplay.Chuzzles.Utils;
using Game.Utility;
using UnityEngine;

#endregion

namespace Game.Gameplay.GamefieldStates
{
    [Serializable]
    public class FieldState : GameState
    {
        public FieldState(Gamefield gamefield) : base(gamefield)
        {
        }

        void OnDestroy()
        {
            TilesCollection.AnimationFinished -= OnAnimationFinished;
        }

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
        public bool IsTurn = false;
        public List<Chuzzle> SelectedChuzzles = new List<Chuzzle>();
        public float TimeFromTip = 0;
        private Chuzzle _arrowChuzzle;
        private bool _axisChozen;

        private bool _hasLockedChuzzles;
        private bool _isReturning;
        private bool _isVerticalDrag;
        private float _maxX;
        private float _maxY;

        private float _minX;
        private float _minY;
        private List<Chuzzle> _possibleCombination;

        private IntVector2 _targetPosition;
        public float returnSpeed = 12f;
        public TipArrow tipArrow;

        public bool IsWorking = true;


        private bool IsVerticalDelta(Vector2 delta)
        {
            return Mathf.Abs(delta.x) < Mathf.Abs(delta.y);
        }

        private bool IsDeltaEnough(Vector2 delta)
        {
            return Mathf.Abs(delta.x) < 1.5*Mathf.Abs(delta.y) || Mathf.Abs(delta.x) > 1.5*Mathf.Abs(delta.y);
        }

        public bool HasLockChuzzles
        {
            get { return SelectedChuzzles.Any(c => c is LockChuzzle); }
        }

        #region Event Handlers

        private void OnAnimationFinished()
        {
            CheckAnimationCompleted();
        }

        public override void OnEnter()
        {
            TilesCollection.AnimationFinished += OnAnimationFinished;
            //  if (!Tutorial.isActive)
            {
                CheckPossibleCombinations();
            }


            /*if (!Gamefield.InvaderWasDestroyed)
            {
                InvaderChuzzle.Populate(Gamefield);
            }
            Gamefield.InvaderWasDestroyed = false;*/
        }

        public override void OnExit()
        {
            TilesCollection.AnimationFinished -= OnAnimationFinished;
            if (TilesCollection.IsAnyAnimated)
            {
                Debug.LogError("FUCK you in field state: " + TilesCollection.AnimatedCount);
            }
            Gamefield.GameMode.HumanTurn();
            Reset();
        }

        #endregion

        private void CheckPossibleCombinations()
        {
            _targetPosition = null;
            _arrowChuzzle = null;
            int numberOfTries = 0;
            do
            {
                if (GamefieldUtility.Repaint(Gamefield.Level.Chuzzles, numberOfTries)) break;

                _possibleCombination = GamefieldUtility.Tip(Gamefield.Level.Chuzzles, out _targetPosition,
                    out _arrowChuzzle);
                // Debug.Log(string.Format("Tip. From: {0} To: {1}", _arrowChuzzle, _targetPosition));
                numberOfTries++;
            } while (!_possibleCombination.Any());
        }

        public void CheckCombinations()
        {
            List<List<Chuzzle>> combinations = GamefieldUtility.FindCombinations(Gamefield.Level.Chuzzles);
            if (combinations.Any())
            {
                foreach (Chuzzle c in Gamefield.Level.Chuzzles)
                {
                    c.MoveTo = c.Current = c.Real;
                }
                Gamefield.SwitchStateTo(Gamefield.PowerUpAnalyzeState);
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
            float size = 0f;
            if (_isVerticalDrag)
            {
                size = Gamefield.Level.Cells.Height*Chuzzle.Scale.y;
            }
            else
            {
                size = Gamefield.Level.Cells.Width*Chuzzle.Scale.x;
            }

            Vector3 difference = CurrentChuzzle.Real.Position - CurrentChuzzle.Current.Position;
            Vector3 velocity = -returnSpeed*difference.normalized;
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

            foreach (Chuzzle c in SelectedChuzzles)
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
        
        public void OnPointerDown(Chuzzle chuzzle)
        {
            if (Gamefield.CurrentState != this)
                return;

            if (!IsWorking)
            {
                return;
            }

            if (_isReturning)
            {
                return;
            }

            CurrentChuzzle = chuzzle;
        }

        private Vector3 ConvertDeltaFromScreenSpaceToWorld(Vector3 delta)
        {
            var viewportWidth = Camera.main.pixelRect.width;
            var chuzzleWidthInPixel = viewportWidth/Gamefield.LevelDescription.Field.Width;
            var chuzzleWidthInUnits = Chuzzle.Scale.x;
            var width = chuzzleWidthInPixel/chuzzleWidthInUnits;
            return delta/width;
        }

        public void OnDrag(Vector2 delta)
        {
          //  Debug.Log("move On chuzzle: " + delta);
            if (!IsWorking)
            {
                return;
            }

            if (Gamefield.CurrentState != this)
                return;

            var clampedDelta = ConvertDeltaFromScreenSpaceToWorld(delta);
            clampedDelta = Vector3.ClampMagnitude(clampedDelta, 0.45f * Chuzzle.Scale.x);

            //Debug.Log("Converted delta: " + clampedDelta);

            if (!_axisChozen)
            {
                ChoseAxis(clampedDelta);
            }

            if (_axisChozen)
            {
                ChoseDragDirection(clampedDelta);
            }

            if (SelectedChuzzles.Any() && _axisChozen && clampedDelta.magnitude >= 0.01f)
            {
                
                //clamp drag
                if (_isVerticalDrag)
                {
                    clampedDelta.x = 0;
                }
                else
                {
                    clampedDelta.y = 0;
                }
                
                foreach (Chuzzle c in SelectedChuzzles)
                {
                    c.transform.position += clampedDelta;
                }

                MoveChuzzles(Gamefield.Level.Cells);

                //move all tiles to new real coordinates
                foreach (Chuzzle chuzzle in SelectedChuzzles)
                {
                    chuzzle.Real = Gamefield.Level.Cells.GetCellAt(GamefieldUtility.ToRealCoordinates(chuzzle), false);
                }

                foreach (var chuzzle in Gamefield.Level.Chuzzles)
                {
                    chuzzle.Tipping = false;
                }

                var combs = GamefieldUtility.SelectedTips(Gamefield.Level.Chuzzles, SelectedChuzzles, 3);
                //Debug.Log("Combs count: "+combs.Count);
                foreach (var comb in combs)
                {
                    foreach (var chuzzle in comb)
                    {
                        chuzzle.Tipping = true;
                    }
                }
            }
        }

        public void OnPointerUp(Chuzzle chuzzle)
        {
            if (Gamefield.CurrentState != this)
                return;

            DropDrag();
        }

        private void ChoseDragDirection(Vector3 delta)
        {
            if (_isVerticalDrag)
            {
                CurrentDirection = delta.y > 0 ? Direction.Top : Direction.Bottom;
                delta.z = delta.x = 0;
            }
            else
            {
                CurrentDirection = delta.x > 0 ? Direction.Right : Direction.Left;
                delta.y = delta.z = 0;
            }
        }

        private void ChoseAxis(Vector3 delta)
        {
            //chooze drag direction
            if (IsDeltaEnough(delta) && CurrentChuzzle)
            {
                if (IsVerticalDelta(delta))
                {
                    SelectedChuzzles = Gamefield.Level.Chuzzles.Where(x => x.Current.X == CurrentChuzzle.Current.X).ToList();
                    //LogChuzzles(SelectedChuzzles);
                    _isVerticalDrag = true;
                }
                else
                {
                    SelectedChuzzles = Gamefield.Level.Chuzzles.Where(x => x.Current.Y == CurrentChuzzle.Current.Y).ToList();
                    //LogChuzzles(SelectedChuzzles);
                    _isVerticalDrag = false;
                }
                /*
                foreach (Chuzzle selectedChuzzle in SelectedChuzzles)
                {
                   selectedChuzzle.Tipping = true;
                }
                */
                _hasLockedChuzzles = HasLockChuzzles;

                if (_hasLockedChuzzles)
                {
                    _minX = CurrentChuzzle.Current.Position.x - Chuzzle.Scale.x*0.4f;
                    _maxX = CurrentChuzzle.Current.Position.x + Chuzzle.Scale.x*0.4f;
                    _minY = CurrentChuzzle.Current.Position.y - Chuzzle.Scale.y*0.4f;
                    _maxY = CurrentChuzzle.Current.Position.y + Chuzzle.Scale.y*0.4f;
                }

                _axisChozen = true;
            }
        }

        private void LogChuzzles(IEnumerable<Chuzzle> selectedChuzzles)
        {
            var s = selectedChuzzles.Aggregate(""+selectedChuzzles.Count()+Environment.NewLine, (current, selectedChuzzle) => current + (selectedChuzzle + Environment.NewLine));
            Debug.Log(s);
        }

        public void LateUpdateState(CellCollection activeCells)
        {
            //tipArrow.UpdateState();
            if (_isReturning)
            {
                foreach (Chuzzle selectedChuzzle in SelectedChuzzles)
                {
                    Vector3 change = selectedChuzzle.Velocity*Time.deltaTime;
                    Vector3 currentPos = selectedChuzzle.transform.position;
                    Vector3 nextPos = currentPos + change;
                    Vector3 moveToPos = selectedChuzzle.MoveTo.Position;
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

                if (SelectedChuzzles.All(x => x.Velocity == Vector3.zero))
                {
                    Reset();
                }
                return;
            }
        }

        private void RevertVelocity()
        {
            foreach (Chuzzle selectedChuzzle in SelectedChuzzles)
            {
                selectedChuzzle.Velocity *= -0.9f;
            }
        }

        private void MoveChuzzles(CellCollection activeCells)
        {
            foreach (Chuzzle c in SelectedChuzzles)
            {
                Vector3 copyPosition = c.transform.position;

                IntVector2 real = GamefieldUtility.ToRealCoordinates(c);
                Cell targetCell = GamefieldUtility.CellAt(activeCells, real.x, real.y);

                Vector3 difference = c.transform.position -
                                     GamefieldUtility.ConvertXYToPosition(real.x, real.y, Chuzzle.Scale);

                bool isNeedCopy = false;

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
                                Cell rightCell = GetRightCell(activeCells, targetCell.Right, c);
                                copyPosition = rightCell.Position + difference - new Vector3(Chuzzle.Scale.x, 0, 0);
                            }
                        }
                        else
                        {
                            isNeedCopy = targetCell.Left == null ||
                                         (targetCell.Left != null && targetCell.Left.Type != CellTypes.Usual);
                            if (isNeedCopy)
                            {
                                Cell leftCell = GetLeftCell(activeCells, targetCell.Left, c);
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
                                Cell topCell = GetTopCell(activeCells, targetCell.Top, c);
                                copyPosition = topCell.Position + difference - new Vector3(0, Chuzzle.Scale.y, 0);
                            }
                        }
                        else
                        {
                            isNeedCopy = targetCell.Bottom == null ||
                                         (targetCell.Bottom != null && targetCell.Bottom.Type == CellTypes.Block);
                            if (isNeedCopy)
                            {
                                Cell bottomCell = GetBottomCell(activeCells, targetCell.Bottom, c);
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

        private void DropDrag()
        {
            if (SelectedChuzzles.Any())
            {
                //drop shining
                foreach (Chuzzle chuzzle in Gamefield.Level.Chuzzles)
                {
                    chuzzle.Tipping = false;
                    chuzzle.Teleportable.Hide();
                }

                //move all tiles to new real coordinates
                foreach (Chuzzle chuzzle in SelectedChuzzles)
                {
                    chuzzle.Real = Gamefield.Level.Cells.GetCellAt(GamefieldUtility.ToRealCoordinates(chuzzle), false);
                }

                foreach (Chuzzle c in Gamefield.Level.Chuzzles)
                {
                    c.MoveTo = c.Real;
                }

                foreach (Chuzzle chuzzle in Gamefield.Level.Chuzzles)
                {
                    ChuzzleMover.Instance.MoveTo(chuzzle, chuzzle.transform.position,chuzzle.MoveTo.Position, 0.1f);
                }

                CheckAnimationCompleted();
            }
        }

        public void Reset()
        {
            foreach (Chuzzle selectedChuzzle in SelectedChuzzles)
            {
                selectedChuzzle.Tipping = false;
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
        }

        public void CheckAnimationCompleted()
        {
            if (!TilesCollection.IsAnyAnimated)
            {
                CheckCombinations();
            }
        }


        public override void LateUpdateState()
        {
            if (TilesCollection.Any())
            {
                LateUpdateState(Gamefield.Level.Cells);
            }
        }

        #region Get Cells

        private static Cell GetBottomCell(IEnumerable<Cell> activeCells, Cell targetCell, Chuzzle c)
        {
//if border
            if (targetCell == null)
            {
                targetCell = GamefieldUtility.CellAt(activeCells, c.Current.X,
                    activeCells.Where(x => !x.IsTemporary).Max(x => x.Y));
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
                    targetCell = GamefieldUtility.CellAt(activeCells, c.Current.X,
                        activeCells.Where(x => !x.IsTemporary).Max(x => x.Y));
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
                targetCell = GamefieldUtility.CellAt(activeCells, c.Current.X,
                    activeCells.Where(x => !x.IsTemporary).Min(x => x.Y));
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
                    targetCell = GamefieldUtility.CellAt(activeCells, c.Current.X,
                        activeCells.Where(x => !x.IsTemporary).Min(x => x.Y));
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
                targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Min(x => x.X), c.Current.Y);
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
                    targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Min(x => x.X),
                        c.Current.Y);
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
                targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Max(x => x.X), c.Current.Y);
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
                    targetCell = GamefieldUtility.CellAt(activeCells, activeCells.Max(x => x.X),
                        c.Current.Y);
                    if (targetCell.Type == CellTypes.Block)
                    {
                        targetCell = targetCell.GetLeftWithType();
                    }
                }
            }
            return targetCell;
        }

        #endregion
    }
}