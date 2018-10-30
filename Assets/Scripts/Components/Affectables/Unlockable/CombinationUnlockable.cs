using System.Collections.Generic;
using UnityEngine;

public class CombinationUnlockable : Unlockable
{
    [SerializeField] private int combination;

    private int[] combinationArray;
    private List<int> currentGuess;

    protected override void Start()
    {
        base.Start();
        combinationArray = GetDigits(combination);
    }

    public void GiveDigit(int number)
    {
        if (currentGuess == null)
        {
            currentGuess = new List<int>();
        }

        if (currentGuess.Count > combinationArray.Length) return;

        if (currentGuess.Count <= 0 && combinationArray[0] == number ||
            currentGuess.Count > 0 && combinationArray[currentGuess.Count] == number)
        {
            currentGuess.Add(number);
        }
        else
        {
            currentGuess = null;
        }

        if (currentGuess == null || currentGuess.Count != combinationArray.Length) return;

        Unlock();
    }

    // This override exists to stop the standard unlock behaviour
    public override void UnlockLock() { }

    private int[] GetDigits(int number)
    {
        string temp = number.ToString();
        int[] digits = new int[temp.Length];
        for (int i = 0; i < digits.Length; i++)
        {
            digits[i] = int.Parse(temp[i].ToString());
        }

        return digits;
    }
}
