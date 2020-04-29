using UnityEngine;
using UnityEngine.UI;

public class StateTest : MonoBehaviour
{
    public Text state;
    public string currentState;
    // Start is called before the first frame update
    void Start()
    {
        currentState = GetComponentInParent<Animations>().state;
        state = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState = GetComponentInParent<Animations>().state;
        state.text = currentState;
    }
}
