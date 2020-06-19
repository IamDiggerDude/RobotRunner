using UnityEngine;
namespace Validator
{
    [CreateAssetMenu(fileName = "Username Validator Rules", menuName = "Validation Rules/Username Validation Rules", order = 0)]
    public class UsernameValidationData : ScriptableObject
    {
        [SerializeField]
        int maxCharacterCount = 16;
        [SerializeField]
        DissalowedCharactersValidationData dissalowedCharacters;
        [SerializeField]
        int minCharacterCount = 6;


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

        public int MinCharacterCount
        {
            get
            {
                return minCharacterCount;
            }

            set
            {
                minCharacterCount = value;
            }
        }
    }
}
