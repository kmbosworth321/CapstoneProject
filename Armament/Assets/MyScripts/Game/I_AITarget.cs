﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Kabaj.TestPhotonMultiplayerFPSGame
{
    /// <summary>
    /// Interface for all targets. Targets are game objects that are affected by getting shot.
    /// Examples: players, boxes that destruct.
    /// </summary>
    interface I_AITarget
    {
        void TakeDamage(float amount);
        void TakeDamage(float amount, AIPlayerManager player);
    }
}
