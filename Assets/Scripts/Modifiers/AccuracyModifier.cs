﻿namespace Modifiers
{
    public class AccuracyModifier : AbstractModifier<OnFireContext>
    {
        protected override void OnSuccess(OnFireContext e)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnFailure(OnFireContext e)
        {
            // TODO: Set this up with different pools for different event types
            throw new System.NotImplementedException();
        }
    }
}