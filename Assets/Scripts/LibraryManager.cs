using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    public List<Book> l_books;

    public void CheckIfComplete()
    {
        if (!GameManager.taskManager.li_taskList.Contains(TaskManager.Task.PutAwayBooks)) return;
        foreach (Book book in l_books)
            if (!book.bl_onShelf) return;

        GameManager.taskManager.CompleteTask(TaskManager.Task.PutAwayBooks);
    }

    public void PutAwayBook(Book book)
    {
        book.bl_onShelf = true;
        CheckIfComplete();
    }

    public void RemoveBook(Book book)
    {
        book.bl_onShelf = false;
        if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.PutAwayBooks) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        GameManager.taskManager.AddTask(TaskManager.Task.PutAwayBooks);
        GameManager.ghost.AddTask(TaskManager.Task.PutAwayBooks);
    }

}
