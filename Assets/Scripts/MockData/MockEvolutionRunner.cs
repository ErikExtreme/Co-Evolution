using System.Collections;
using UnityEngine;

public class MockEvolutionRunner : MonoBehaviour
{
    public EvolutionManager evolutionManager;

    IEnumerator Start()
    {
        // Make results deterministic
        Random.InitState(12345);

        // Run several mock Evolve() calls
        for (int i = 0; i < 5; i++)
        {
            evolutionManager.Evolve();
            yield return null;
        }

        // Quit so OnApplicationQuit() saves the log
        Application.Quit();
    }
}
