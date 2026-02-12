using UnityEngine;
using System.Collections;

public abstract class FSM : MonoBehaviour
{
    protected abstract void FSM_Initialize();
    protected abstract void FSMTick();
    protected abstract void FSM_FixedTick();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FSM_Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        FSMTick();
    }

    private void FixedUpdate()
    {
        FSM_FixedTick();
    }
}
