using UnityEngine;
using System.Collections;

public abstract class FSM : MonoBehaviour
{
    protected abstract void Initialize();
    protected abstract void FSMUpdate();
    protected abstract void FSMFixedUpdate();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        FSMUpdate();
    }

    private void FixedUpdate()
    {
        FSMFixedUpdate();
    }
}
