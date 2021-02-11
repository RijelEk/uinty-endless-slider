using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu
{

}

public class RectScrollScript : MonoBehaviour
{
    
    [Header("Game Objects")]
    public GameObject Content;
    public GameObject NavItemPrefab;
    public GameObject accent;
    public GameObject sectionName;
    public GameObject sectionDescriptionTexMeshPro;
    public GameObject subSectionPrefab;
    public GameObject LeftUpperMenu;

    [Header("Inventory")]
    public GameObject Inventory;

    [Header("Menu Sections")]
    public GameObject Alchemy;

    [Header("Controllers")]
    [Range(0f, 1f)]
    public float speed = 0.3f; //suggested 0.3f

    [Header("Scale size")]
    public float scaleSize = 1.5f;


    private bool mouseDown, buttonDown, buttonUp;
    private bool menuActive = false;
    private string left, center, right, first, last;
    private List<string> navItems = new List<string>() { "Tutorials", "Character", "Books", "Diary", "Inventory", "Alchemy", "Quests", "Map" };
    private List<string> activeItems = new List<string>() {}; //choisen active items
    private List<GameObject> instPans = new List<GameObject>(); //instantiated navigation items
    private int TotalActiveItems = 6; // 5 istantiated and 1 added

    private bool loading = false; // scrolling animation progress

    private Color32 PaleGray = new Color32(109, 110, 106, 255);
    private Color32 Gray = new Color32(79, 80, 77, 255);
    private Color32 Brown = new Color32(106, 65, 16, 255);

  
    private List<GameObject> subsections = new List<GameObject>();


    /* -------         (*) Instantiate Subsections      -------*/
    private void InstSubSections(string subsectionName)
    {
        string section1 = "";
        string section2 = "";
        bool exist = false;

        switch (subsectionName)
        {
            case "Alchemy":
                section1 = "INVENTORY";
                section2 = "SCHEMATICS";
                exist = true;
                break;
            default:
                exist = false;
                break;
        }

        if (exist)
        {
            subsections.Insert(0, Instantiate(subSectionPrefab, transform, false));
            subsections[0].transform.SetParent(LeftUpperMenu.transform, false);
            subsections[0].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = section1;
            subsections[0].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = section2;
        }

    }

        /* -------         (*) Set Menu Subsections      -------*/
        private void SetMenuSubSections(string subsectionName)
    {   
        if(subsections.Count <= 0)
        {
            InstSubSections(subsectionName);

        } else
        {
            Destroy(subsections[0]);
            subsections.Clear();
            InstSubSections(subsectionName);
        }
    }





    /* -------         (*) Set Position of Inst Item        -------*/
    private void position(GameObject obj, float x, float y)
    {
        obj.transform.localPosition = new Vector2(x, y);
    }

    /* -------          (*) Instantiate Item        -------*/
    private GameObject create(string name, int index, GameObject parent)
    {
        instPans.Insert(index, Instantiate(NavItemPrefab, transform, false));
        instPans[index].name = name;
        instPans[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
   
        instPans[index].transform.SetParent(Content.transform, false);

        if (index == 2)
        {
            sectionName.transform.GetComponent<TextMeshProUGUI>().text = name.ToUpper();
         
            instPans[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Brown;
            instPans[index].transform.localScale = new Vector3(scaleSize, scaleSize, 0);

       
            GameObject MenuSubsectionManager = GameObject.Find("Left Upper Menu Manager");
            MenuSubsectionManager.GetComponent<SetSectionNameAndDescription>().SetMenuDescription(instPans[index].name); //Set section name
            MenuSubsectionManager.GetComponent<Instantiate>().SetMenuSubSections(instPans[index].name); //Set Subsection

            GameObject MenuSections = GameObject.Find("Menu Sections"); // Activate menu
            MenuSections.GetComponent<MenuSectionsManager>().SetActiveSection(name);


        } else if(index == 0)
        {
              instPans[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = PaleGray;
        } else if (index == 4)
        {
            instPans[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Gray;
        }


        return instPans[index];
    }

 

    /* -------          Instantiate First 3 Objects on Start        -------*/
    private void InstantiateObjects()
    {
        

        if (activeItems.Count == 5)
        {
            for(int i =0; i< activeItems.Count(); i++)
            {
                create(activeItems[i], i , Content);
                if (i == 0) continue;
                position(instPans[i], instPans[i - 1].transform.localPosition.x + NavItemPrefab.GetComponent<RectTransform>().sizeDelta.x, instPans[i].transform.localPosition.y);
            }
        }
    }

    /* -------          Instantiate additional Object on Scroll        -------*/
    private void InstaniateOneObject(string name, bool end)
    {
       
        int index = end ? instPans.Count : 0;

        create(name, index, Content);
        float x = !end ? 
            instPans[index+1].transform.localPosition.x - NavItemPrefab.GetComponent<RectTransform>().sizeDelta.x
            : instPans[index-1].transform.localPosition.x + NavItemPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float y = instPans[index].transform.localPosition.y;

  
        position(instPans[index], x, y);
    }

    public void AddOnStart(string active)
    {
      

       for (int i = 0; i < navItems.Count; i++)
        {
            if (navItems[i] == active)
            {
              
                first = i - 2 > 0 ? navItems[i - 2] : navItems[navItems.Count - 2];
                left = i - 1 > 0 ? navItems[i - 1] : navItems[navItems.Count - 1];
                center = navItems[i];
                right = i + 1 <= navItems.Count - 1 ? navItems[i + 1] : navItems[0];
                last = i + 2 <= navItems.Count - 1 ? navItems[i + 2] : i + 2 == navItems.Count  ?  navItems[0] : navItems[1];

                activeItems.Insert(0, first);
                activeItems.Insert(1, left);
                activeItems.Insert(2, center);
                activeItems.Insert(3, right);
                activeItems.Insert(4, last);

              

                InstantiateObjects();
                menuActive = true;
            }
        };

    }

    /* -------          Set Active Items on Start        -------*/
    public void SetActive(string active)
    {


        activeItems.Clear();
        AddOnStart(active);
        if (activeItems.Count > 0 && instPans.Count > 0)
        {
            Debug.Log("Somethign exist");
            if (active != instPans[1].name)
            {
                GameObject MenuSections = GameObject.Find("Menu Sections"); // Activate menu
                MenuSections.GetComponent<MenuSectionsManager>().SetInactiveSection(active);
                Debug.Log("Instantiated");
                activeItems.Clear();
                for (int i = 0; i < instPans.Count; i++)
                {
                    Destroy(instPans[i].gameObject);
                }
                instPans.Clear();
             
                AddOnStart(active);
            }
            else
            {
                Debug.Log("Not Instantiated");
                activeItems.Clear();
                AddOnStart(active);
            }

        }
        else
        {
            Debug.Log("Does not exist");
            activeItems.Clear();
            AddOnStart(active);
        }

    }


    /* -------         Navigation Direction dan Activation        -------*/
    void Update()
    {
       
        /*Check if menu is open to navigate*/
        if (menuActive)
        {

            /*Button Navigation*/
            if (mouseDown)
            {
                Debug.Log("MOUAE DOWN");
                if (buttonDown)
                {
                    ScrollDown();
                }
                else if (buttonUp)
                {
                    ScrollUp();
                }
            }
        }
       
    }

    public void ButtonDownIsPressed()
    {
      
        mouseDown = true;
        buttonDown = true;
    }

    public void ButtonUpIsPressed()
    {
    
        mouseDown = true;
        buttonUp = true;
    }


    private int next(string current, bool forward)
    {
        int nextIndex = 0;
        for (int i = 0; i < navItems.Count; i++)
        {
            if (navItems[i] == current)
            {

                if (!forward)
                {
    
                    nextIndex = i < navItems.Count-1 ? i + 1 : 0;

                }
                else
                {
                    nextIndex = i != 0 ? i - 1 : navItems.Count - 1;
                }
                break;
            }
        }
        return nextIndex;
    }

    /* -------          Destroy object               -------*/

    private void destroy(GameObject element)
    {
        instPans.Remove(element);
        Destroy(element.gameObject);
    }

    /* -------          Scale Nav Items       -------*/

    private void scaleUp(GameObject obj)
    {
       
        StartCoroutine(scaleTimingUp(speed, obj));
    }

    private void scaleDown(GameObject obj)
    {
      
        StartCoroutine(scaleTimingDown(speed, obj));
    }

    IEnumerator scaleTimingUp(float LerpTime, GameObject obj)
    {
        float StartTime = Time.time;
        float EndTime = StartTime + LerpTime;

        while (Time.time < EndTime)
        {
            float timeProgressed = (Time.time - StartTime) / LerpTime + 0.5f;  // this will be 0 at the beginning and 1 at the end.

            if (timeProgressed > 0.9)
            {
                timeProgressed = 1;
            } 


            obj.transform.localScale = new Vector3(timeProgressed*scaleSize, timeProgressed * scaleSize, 0);

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator scaleTimingDown(float LerpTime, GameObject obj)
    {
        float StartTime = Time.time;
        float EndTime = StartTime + LerpTime;

        while (Time.time < EndTime)
        {
            float timeProgressed = (Time.time - StartTime) / LerpTime;  // this will be 0 at the beginning and 1 at the end.
            

            if (timeProgressed > 0.9)
            {
                timeProgressed = 1;
            }

            float resultSize = scaleSize - timeProgressed > 1 ? scaleSize - timeProgressed : 1;


            obj.transform.localScale = new Vector3(resultSize, resultSize, 0);

            yield return new WaitForFixedUpdate();
        }
    }


    /* -------          Scrolling Main actions       -------*/


    private void Scrolling(bool forward)
    {
       
        /*   -- Select next Item acc to direction -- */
        if (!loading){
              /* 1. Current element */
              loading = true;
        int index = !forward ? activeItems.Count - 1 : 0;
        string current = activeItems[index];
      
        /* 2. Find in array of all elements its index */
        int nextIndex = next(current, forward);
        
        /*   Create One more item acc to direction in particular position   */
        string NextElement = navItems[nextIndex];
       

        float distance = instPans[1].transform.position.x - instPans[0].transform.position.x;

        InstaniateOneObject(NextElement, !forward);

            /*   Move Nav items    */

            

            for (int i = 0; i < TotalActiveItems; i++)
        {

            GameObject El = instPans[i];

             float posX = El.transform.position.x;
             float posY = El.transform.position.y;

             float posXEnd = forward ? posX + distance : posX - distance;

             Vector2 PosStart = new Vector2(posX, posY);
             Vector2 PosEnd = new Vector2(posXEnd, posY);
               
                if(current == instPans[i].name)
                {
                    if (forward)
                    {
                       
                        instPans[i+3].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = PaleGray;
                        instPans[i+2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Gray;
                        instPans[i + 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Brown; //CURRENT
                        instPans[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Gray;
                        instPans[i-1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = PaleGray;

                        scaleUp(instPans[i + 1]);
                        scaleDown(instPans[i + 2]);

                        sectionName.transform.GetComponent<TextMeshProUGUI>().text = instPans[i + 1].name.ToUpper(); //set menu name

                        GameObject MenuSubsectionManager = GameObject.Find("Left Upper Menu Manager");
                        MenuSubsectionManager.GetComponent<SetSectionNameAndDescription>().SetMenuDescription(instPans[i+1].name); //Set section name
                
                        SetMenuSubSections(instPans[i + 1].name); //set subsections

                        GameObject MenuSections = GameObject.Find("Menu Sections"); // Activate menu
                        MenuSections.GetComponent<MenuSectionsManager>().SetActiveSection(instPans[i + 1].name);
                        MenuSections.GetComponent<MenuSectionsManager>().SetInactiveSection(instPans[i + 2].name);


                    } else
                    {

                        instPans[i - 3].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = PaleGray;
                        instPans[i-2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Gray;
                        instPans[i -1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Brown; //CURRENT
                        instPans[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Gray;
                        instPans[i + 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = PaleGray;

                        scaleUp(instPans[i - 1]);
                        scaleDown(instPans[i - 2]);

                        sectionName.transform.GetComponent<TextMeshProUGUI>().text = instPans[i - 1].name.ToUpper(); //set menu name

                        GameObject MenuSubsectionManager = GameObject.Find("Left Upper Menu Manager");
                        MenuSubsectionManager.GetComponent<SetSectionNameAndDescription>().SetMenuDescription(instPans[i - 1].name); //Set section name
        
                        SetMenuSubSections(instPans[i - 1].name); //set subsections

                        GameObject MenuSections = GameObject.Find("Menu Sections"); // Activate menu
                        MenuSections.GetComponent<MenuSectionsManager>().SetActiveSection(instPans[i - 1].name);
                        MenuSections.GetComponent<MenuSectionsManager>().SetInactiveSection(instPans[i - 2].name);

                    }
                  
                }
               

            El.GetComponent<MoveNavItemScript>().runScript(PosStart, PosEnd, speed);
           
        }

           

            StartCoroutine(ExecuteAfterTime(speed));

    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
            /*  Destroy Item  */
            int indexToDestory = forward ? 5 : 0;
            destroy(instPans[indexToDestory]);

            /* Rearrange List of Instantiated and Active objects */
            activeItems.Clear();
            for (int i = 0; i < instPans.Count; i++)
            {
                activeItems.Add(instPans[i].name);
            }
            loading = false;
        }
        // Code to execute after the delay
      
    }

    private void ScrollDown()
    {
       
        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            buttonDown = false;
            buttonUp = false;
        } else
        {
            /* Move object */
    
            mouseDown = false;
            buttonUp = false;
            buttonDown = false;
            Scrolling(true);
        }
    }

    private void ScrollUp()
    {
     
        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            buttonUp = false;
        }
        else
        {
            /* Move object */
            mouseDown = false;
            buttonUp = false;
            Scrolling(false);
        }
    }


}
