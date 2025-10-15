using Godot;
using System;
using System.Collections.Generic;  // : List, Dictionary
using System.Linq;                 // : Enumerable

/// Iterates through a shuffled list of items, which is Reinitialized at a threshold.
public class MyLoopingQueue
{
    private List<string> _template;
    private List<string> _queue;
    public int Threshold;
    public bool Shuffle;

    public MyLoopingQueue(
        Dictionary<string, int> setup,
        int threshold = 0,
        bool shuffle = true
    ) {
        Threshold = threshold;
        Shuffle = shuffle;
        _template = new List<string>();

        // Adds as many of the string as specified by int
        foreach (KeyValuePair<string, int> e in setup)
        {
            _template.AddRange(Enumerable.Repeat(e.Key, e.Value));
        }

        Reinitialize();
    }

    private void Reinitialize()
    {
        if (Shuffle)
        {
            // Use a temp array to take advantage of built-in shuffle
            string[] buffer = _template.ToArray();
            Random.Shared.Shuffle(buffer);
            _queue = new List<string>(buffer);
        }
        else
        {
            _queue = new List<string>(_template);
        }
    }

    /// Retrieve and remove one item; shuffle if beneath threshold
    public string Pop()
    {
        string item;
        // Reload queue and shuffle if under threshold
        if (_queue.Count <= Threshold)
        {
            Reinitialize();
        }
        // Pop the item
        item = _queue[0];
        _queue.RemoveAt(0);
        return item;
    }

    /// Get a string representation of the queue as is
    public string Repr()
    {
        if (_queue != null)
        {
            string str = "MyLoopingQueue: {\n";
            foreach (var e in _queue)
            {
                str += $"    {e}\n";
            }
            return str + "}\n";
        }
        else
        {
            return "null";
        }

    }
}
