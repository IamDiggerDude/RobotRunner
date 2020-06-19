using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Validator
{

    [CreateAssetMenu(fileName = "Dissalowed Characters Validator Rules", menuName = "Validation Rules/Dissalowed Characters Validation Rules", order = 51)]
    public class DissalowedCharactersValidationData : ScriptableObject
    {
        [Tooltip("Fill string using \",\" (i.e.: @,3,I,i)")]
        [SerializeField]
        string disallowedCharacters = "";
        string[] Splitter = { "," };
        public string[] DisallowedCharacters
        {
            get
            {
                return disallowedCharacters.Split(Splitter, StringSplitOptions.None);
            }
        }

    }
}