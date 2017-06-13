using System;
using StudioX.Runtime.Validation;

namespace StudioX.UI.Inputs
{
    [Serializable]
    [InputType("CHECKBOX")]
    public class CheckboxInputType : InputTypeBase
    {
        public CheckboxInputType()
            : this(new BooleanValueValidator())
        {

        }

        public CheckboxInputType(IValueValidator validator)
            : base(validator)
        {
            
        }
    }
}