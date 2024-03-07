using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Calendar : MonoBehaviour
{
    DateTime dateTime = DateTime.Now;
    int day;
    string month;

    [SerializeField] TMP_Text tmp_day;
    [SerializeField] TMP_Text tmp_month;

    void Update()
    {
        day = dateTime.Day;
        switch (dateTime.Month)
        {
            case 1:
                month = "January";
                break;
            case 2:
                month = "February";
                break;
            case 3:
                month = "March";
                break;
            case 4:
                month = "April";
                break;
            case 5:
                month = "May";
                break;
            case 6:
                month = "June";
                break;
            case 7:
                month = "July";
                break;
            case 8:
                month = "August";
                break;
            case 9:
                month = "September";
                break;
            case 10:
                month = "October";
                break;
            case 11:
                month = "November";
                break;
            case 12:
                month = "December";
                break;
        }

        tmp_day.text = day.ToString();
        tmp_month.text = month;
    }
}