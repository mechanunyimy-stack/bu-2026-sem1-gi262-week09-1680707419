
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solution
{
    public class ActionHistoryManager : MonoBehaviour
    {
        // 1. Undo System using Stack
        private Stack<Vector2> undoStack = new Stack<Vector2>();
        private Stack<Vector2> redoStack = new Stack<Vector2>();

        // 2. Auto-Move System using Queue
        private Queue<Vector2> autoMoveQueue = new Queue<Vector2>();

        #region "This Is undoStack Function"

        // Save current player position
        public void SaveStateForUndo(Vector2 currentPosition)
        {
            // Save only if position is different
            if (undoStack.Count == 0 || undoStack.Peek() != currentPosition)
            {
                undoStack.Push(currentPosition);

                // Clear redo history when a new move is made
                redoStack.Clear();
            }
        }

        // Undo last move
        public void UndoLastMove(OOPPlayer player)
        {
            // Need at least two states
            if (undoStack.Count < 2)
            {
                return;
            }

            // Move current state to redo stack
            Vector2 currentPosition = undoStack.Pop();
            redoStack.Push(currentPosition);

            // Get previous position
            Vector2 prevPosition = undoStack.Peek();

            // Update player position
            player.UpdatePosition(
                (int)prevPosition.x,
                (int)prevPosition.y
            );
        }

        // Redo last undone move
        public void RedoLastMove(OOPPlayer player)
        {
            if (redoStack.Count == 0)
            {
                return;
            }

            // Restore the previous state
            Vector2 position = redoStack.Pop();
            undoStack.Push(position);

            // Update player position
            player.UpdatePosition(
                (int)position.x,
                (int)position.y
            );
        }

        #endregion


        #region "This Is autoMoveQueue Function"

        // Start automatic movement
        public void StartAutoMoveSequence(OOPPlayer player)
        {
            // Prepare movement sequence
            Vector2[] moves = new Vector2[]
            {
                Vector2.right,
                Vector2.right,
                Vector2.right,
                Vector2.right,
                Vector2.up,
                Vector2.left,
                Vector2.up,
                Vector2.down
            };

            // Clear old queue
            autoMoveQueue.Clear();

            // Add moves to queue
            for (int i = 0; i < moves.Length; i++)
            {
                autoMoveQueue.Enqueue(moves[i]);
            }

            // Start coroutine
            StartCoroutine(ProcessAutoMoveSequence(player));
        }

        // Process automatic movement
        public IEnumerator ProcessAutoMoveSequence(OOPPlayer player)
        {
            player.isAutoMoving = true;

            Debug.Log(
                $"Auto-move sequence started with {autoMoveQueue.Count} steps."
            );

            // Process queue step by step
            while (autoMoveQueue.Count > 0)
            {
                yield return new WaitForSeconds(0.5f);

                // Get next movement
                Vector2 move = autoMoveQueue.Dequeue();

                // Get current player position
                Vector3 currentPosition = player.transform.position;

                // Calculate next position
                int newX = (int)currentPosition.x + (int)move.x;
                int newY = (int)currentPosition.y + (int)move.y;

                // Update player position
                player.UpdatePosition(newX, newY);
            }

            player.isAutoMoving = false;

            Debug.Log("Auto-move sequence finished.");
        }

        #endregion
    }
}


