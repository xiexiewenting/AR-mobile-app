using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{/* This is the main loading screen!  */
    // Start is called before the first frame update
    void Start(){}

    // Update is called once per frame
    void Update(){}

    public void LoadPart1() {
        SceneManager.LoadScene("Part1");
    }

    public void LoadPart2() {
        SceneManager.LoadScene("Part2");
    }

    public void LoadPart3() {
        SceneManager.LoadScene("Part3");
    }
}
