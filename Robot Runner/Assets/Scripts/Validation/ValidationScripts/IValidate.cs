using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Interface for fields validation purposes.
/// Class that inherits it should be used as validator.
/// </summary>
namespace Validator
{
    public interface IValidate
    {
        bool isValid();
    }
}
