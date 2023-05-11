using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_HasProgress
{
    //event for progressBar
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs
    {
        //normalized means that value is from 0 to 1. And events usually called begining in ON
        public float progressNormalized;
    }
}
