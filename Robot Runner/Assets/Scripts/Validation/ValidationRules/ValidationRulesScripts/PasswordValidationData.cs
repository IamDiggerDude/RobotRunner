using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Validator
{
    [CreateAssetMenu(fileName = "Password Validator Rules", menuName = "Validation Rules/Password Validation Rules", order = 1)]
    public class PasswordValidationData : ScriptableObject
    {
        [SerializeField]
        int minCharactersCount = 6;
        [SerializeField]
        int maxCharactersCount = 16;
        [SerializeField]
        bool requestCapitalLetters = true;
        [SerializeField]
        bool requestDigits = true;

        public int MinCharactersCount
        {
            get
            {
                return minCharactersCount;
            }

            set
            {
                minCharactersCount = value;
            }
        }

        public int MaxCharactersCount
        {
            get
            {
                return maxCharactersCount;
            }

            set
            {
                maxCharactersCount = value;
            }
        }

        public bool RequestCapitalLetters
        {
            get
            {
                return requestCapitalLetters;
            }

            set
            {
                requestCapitalLetters = value;
            }
        }

        public bool RequestDigits
        {
            get
            {
                return requestDigits;
            }

            set
            {
                requestDigits = value;
            }
        }
    }
}
