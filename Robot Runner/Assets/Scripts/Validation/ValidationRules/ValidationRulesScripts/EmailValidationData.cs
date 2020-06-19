using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Validator
{
    [CreateAssetMenu(fileName = "Email Validator Rules", menuName = "Validation Rules/Email Validation Rules", order = 2)]
    public class EmailValidationData : ScriptableObject
    {
        [SerializeField]
        int maxCharacterCount = -1;
        [SerializeField]
        DissalowedCharactersValidationData dissalowedCharacters;

        public int MaxCharacterCount
        {
            get
            {
                return maxCharacterCount;
            }
            set
            {
                maxCharacterCount = value;
            }
        }

        public DissalowedCharactersValidationData DissalowedCharacters
        {
            get
            {
                return dissalowedCharacters;
            }

            set
            {
                dissalowedCharacters = value;
            }
        }
    }
}
