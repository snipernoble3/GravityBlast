using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour {

    [SerializeField] CursorLockMode lockMode;
    [SerializeField] bool visible;

    void Awake () {
        UpdateCursor();
    }

    private void UpdateCursor () {
        Cursor.lockState = lockMode;
        Cursor.visible = visible;
    }

    //both
    public void SetCursor (CursorLockMode l, bool v) {
        lockMode = l;
        visible = v;
        UpdateCursor();
    }

    //lock mode only
    public void SetCursor (CursorLockMode l) {
        lockMode = l;
        UpdateCursor();
    }

    //visibility only
    public void SetCursor (bool v) {
        visible = v;
        UpdateCursor();
    }
}
