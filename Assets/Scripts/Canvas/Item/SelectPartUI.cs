using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPartUI : ViewController {

    public Text nameLabel;
    //public SelectOptionUI selectOptionUI;
    public Transform optionContents;
    //public SelectOptionUI selectOpt;

    // Use this for initialization
    void Start () {
        //Debug.Log("option contents : " + optionContents.name);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // 선택가능한 옵션 Object 생성 및 컬러 설정
    public void Setup(CustomizingPart part)
    {
        nameLabel.text = part.name;
        List<CustomizingOption> options = JSONHandler.GetOptionsListByPartId(part.id);
        optionContents = transform.Find("SelectOptions/Viewport/Content");
        Debug.Log(optionContents.name);

        foreach ( CustomizingOption option in options)
        {
            Debug.Log(option.name);
            GameObject optionUI = UIManager.Instance.selectOptionObjPool.GetObject();

            // 각 옵션 클릭했을 때의 리스너 설정
            optionUI.GetComponent<Button>().onClick.AddListener(() => {
                part.GetMaterial().mainTexture = option.GetTexture();
                // 다른 option들의 체크 해제
                //foreach(CheckBox checkbox in optionUI.transform.parent.GetComponentsInChildren<CheckBox>()){ checkbox.SetChecked(false);}
                //optionUI.GetComponentInChildren<CheckBox>().SetChecked(true);
            });

            optionUI.GetComponent<Image>().color = option.GetColorByRGB();
            optionUI.transform.SetParent(optionContents, false);
            //optionUI.transform.SetParent(selectOpt.optionContents, false);

            // 각 파트별로 첫번쨰 옵션으로 초기화.
            if (option == options[0]){
                //optionUI.GetComponentInChildren<CheckBox>().SetChecked(true);
                part.GetMaterial().mainTexture = option.GetTexture();
            }
        }

    }
}
