using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class WrapTextMesh : MonoBehaviour
{
    public float wrapWidth;
    [TextArea(5, 20)]
    public string startingText;

    TextMesh textCache = null;

    public string Text
    {
        get
        {
            return startingText;
        }
        set
        {
            Label.text = value;
            Wrap(wrapWidth);
        }
    }

    public TextMesh Label
    {
        get
        {
            if(textCache == null)
            {
                textCache = GetComponent<TextMesh>();
            }
            return textCache;
        }
    }
    
    public void Wrap(float MaxWidth)
    {
        if(Label == null)
        {
            Debug.LogError("TextMesh component not found.");
            return;
        }
        
        Font f = Label.font;
        string str = Label.text;
        int nLastWordInd = 0;
        int nIter = 0;
        float fCurWidth = 0.0f;
        float fCurWordWidth = 0.0f;
        while (nIter < str.Length)
        {
            // get char info
            char c = str[nIter];
            CharacterInfo charInfo;
            if (c == '\n')
            {
                nLastWordInd = nIter;
                fCurWidth = 0.0f;
            }
            else if (c == '\r')
            {
                ++nIter;
                continue;
            }
            else if (!f.GetCharacterInfo(c, out charInfo))
            {
                Debug.LogError("Unrecognized character encountered (" + (int)c + "): " + c);
                return;
            }
            else
            {
                if (c == ' ')
                {
                    nLastWordInd = nIter; // replace this character with '/n' if breaking here
                    fCurWordWidth = 0.0f;
                }
                
                fCurWidth += charInfo.width;
                fCurWordWidth += charInfo.width;
                if (fCurWidth >= MaxWidth)
                {
                    str = str.Remove(nLastWordInd, 1);
                    str = str.Insert(nLastWordInd, "\n");
                    fCurWidth = fCurWordWidth;
                }
            }
            
            ++nIter;
        }
        
        Label.text = str;
    }
    
    // Use this for initialization
    void Start()
    {
        if(string.IsNullOrEmpty(startingText) == false)
        {
            Text = startingText;
        }
    }
}
