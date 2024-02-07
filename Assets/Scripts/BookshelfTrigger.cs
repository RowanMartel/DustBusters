using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookshelfTrigger : MonoBehaviour
{

    public LibraryManager libraryManager;

    private void OnTriggerEnter(Collider other)
    {

        Book book = other.GetComponent<Book>();

        if(book != null)
        {
            libraryManager.PutAwayBook(book);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Book book = other.GetComponent<Book>();

        if(book != null)
        {
            libraryManager.RemoveBook(book);
        }
    }

}
