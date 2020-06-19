using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Validator
{
    [CreateAssetMenu(fileName = "Colorizer Colors",menuName = "Validation Rules/Colorizer Colors",order=101)]
    public class ColorizerData : ScriptableObject
    {
        [SerializeField]
        Color normalColor = Color.white;
        [SerializeField]
        Color invalidColor = Color.red;
        [SerializeField]
        Color validColor = Color.white;

        public Color NormalColor { get => normalColor; set => normalColor = value; }
        public Color InvalidColor { get => invalidColor; set => invalidColor = value; }
        public Color ValidColor { get => validColor; set => validColor = value; }
    }
}
