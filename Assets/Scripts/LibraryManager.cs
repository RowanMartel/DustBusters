using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    public List<Book> l_books;

    //Checks if all books are in the triggers
    public void CheckIfComplete()
    {
        int int_notOnShelf = 0;
        if (!GameManager.taskManager.li_taskList.Contains(TaskManager.Task.PutAwayBooks)) return;
        foreach (Book book in l_books)
            if (!book.bl_onShelf)
            {
                int_notOnShelf++;
            }
        if(int_notOnShelf > 0)
        {
            GameManager.taskManager.UpdateTask(l_books.Count - int_notOnShelf, TaskManager.Task.PutAwayBooks);
            return;
        }

        GameManager.taskManager.CompleteTask(TaskManager.Task.PutAwayBooks);
    }

    //Recognize that a book was put in a trigger, then check if complete
    public void PutAwayBook(Book book)
    {
        book.bl_onShelf = true;
        CheckIfComplete();
    }

    //Recognize that a book was removed from a trigger, then uncomplete the task if necessary
    public void RemoveBook(Book book)
    {
        CheckIfComplete();
        book.bl_onShelf = false;
        if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.PutAwayBooks) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        GameManager.taskManager.AddTask(TaskManager.Task.PutAwayBooks);
        GameManager.ghost.AddTask(TaskManager.Task.PutAwayBooks);
    }

}
