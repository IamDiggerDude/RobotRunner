using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Validator
{
    public class TMPFieldColorizer
    {
        ColorizerData colorizerData;
        
        public TMPFieldColorizer(ColorizerData colorizerData)
        {
            this.colorizerData = colorizerData;
        }
        //on edit begin reset colors
        public void AddOnSelectHandler(TMP_InputField inputField)
        {
            inputField.onSelect.AddListener((text) => OnSelect(inputField));
        }
        void OnSelect(TMP_InputField inputField)
        {
            //make wayaround thanks to CS1612 error
            ColorBlock block = inputField.colors;
            block.normalColor = colorizerData.NormalColor;     
            inputField.colors = block;
        }
        public void ColorInputField(TMP_InputField inputField, bool isValid)
        {
            ColorBlock block = inputField.colors;
            if (isValid)
            {
                block.normalColor = colorizerData.ValidColor;
            }
            else
            {
                block.normalColor = colorizerData.InvalidColor;
            }
            inputField.colors = block;
        }
    }
}
