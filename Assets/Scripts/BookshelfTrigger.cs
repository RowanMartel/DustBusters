using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookshelfTrigger : MonoBehaviour
{

    public LibraryManager libraryManager;

    //Tells library manager when a book enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        Book book = other.GetComponent<Book>();

        if(book != null)
        {
            libraryManager.PutAwayBook(book);
        }
    }

    //Tells library manager when a book leaves the trigger
    private void OnTriggerExit(Collider other)
    {
        Book book = other.GetComponent<Book>();

        if(book != null)
        {
            libraryManager.RemoveBook(book);
            libraryManager.CheckIfComplete();
        }
    }

}
