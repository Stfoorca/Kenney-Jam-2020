using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursable : MonoBehaviour
{
    public enum Curse
    {
        Fear,
        ZabaPoly,
        RybaPoly,
        PszczolaPoly,
        SlimePoly,
        Charm,
        Control,
        Petrify
    }
    public List<Curse> curses = new List<Curse>();
    
    public void AddCurse(Curse curse)
    {
        if(!curses.Contains(curse))
            curses.Add(curse);
    }

    public void HandleCurse()
    {
        foreach (Curse curse in curses)
        {
            
        }
    }
}
