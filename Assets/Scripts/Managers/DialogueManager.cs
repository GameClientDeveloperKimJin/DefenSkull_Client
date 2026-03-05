using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IPointerDownHandler
{
    public Text DialogueText;
    public GameObject NextTextObj;
    public CanvasGroup DialogueGroup;

    public Queue<string> Sentences;
    public float typingSpeed = 0.05f;
    public bool isTyping;
    public static bool isPlayerSkill;//플레이어 스킬 여부

    private string currentSentence; // 스펠링 수정

    public static DialogueManager instance;

    public GameObject NPC;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Sentences = new Queue<string>();
        DialogueGroup.alpha = 0;
        DialogueGroup.blocksRaycasts = false;
    }

    public void OnDialogue(string[] lines)
    {
        Sentences.Clear();

        foreach (string line in lines)
        {
            Sentences.Enqueue(line);
        }

        DialogueGroup.alpha = 1;
        DialogueGroup.blocksRaycasts = true;
        NextSentence();
    }

    public void NextSentence()
    {
        if (Sentences.Count > 0)
        {
            currentSentence = Sentences.Dequeue();
            isTyping = true;
            StartCoroutine(Typing(currentSentence));
        }
        else
        {
            //대화 종료
            isPlayerSkill = true;
            NPC.GetComponent<NPC>().isTalk = false;
            NPC.GetComponent<NPC>().AIShootOption(false);
            DialogueGroup.alpha = 0;
            DialogueGroup.blocksRaycasts = false;
        }
    }

    IEnumerator Typing(string line)
    {
        DialogueText.text = "";
        NextTextObj.SetActive(false);

        foreach (char letter in line.ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        // 타이핑이 완료된 후 다음 텍스트 오브젝트 활성화
        NextTextObj.SetActive(true);
        isTyping = false;
    }

    void Update()
    {
        // 대사가 다 출력되었는지 확인
        if (DialogueText.text.Equals(currentSentence) && !isTyping) //대사 한줄 끝의 조건임. True면 다음 대사로 넘어감
        {
            NextTextObj.SetActive(true); //타이핑 끝 일때는 "다음" 텍스트 활성화
            isTyping = false;
        }
    }

    public void Next()
    {
        Debug.Log("버튼 클릭");
        NextSentence();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // 타이핑 중일 때는 클릭 감지를 무시
        //if (isTyping)
        //{
        //    // 클릭 이벤트를 아예 처리하지 않도록 설정
        //    return; // 아무 작업도 하지 않음
        //}

        NextSentence();
        // 타이핑이 끝난 경우 다음 문장으로 넘어감

    }
}
