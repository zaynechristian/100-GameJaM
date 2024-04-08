using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateBase
{
    public abstract void EnterState(PlayerStateManager _PSM);

    public abstract void FixedUpdateState(PlayerStateManager _PSM);

    public abstract void UpdateState(PlayerStateManager _PSM);


    public abstract void LateUpdateState(PlayerStateManager _PSM);
}
