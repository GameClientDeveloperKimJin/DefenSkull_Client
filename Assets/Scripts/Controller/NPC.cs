using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;



[System.Serializable]
public class DialogueEntry
{
    public string dialogue;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueEntry> Firstdialogues;
    public List<DialogueEntry> Seconddialogues;
}

public class NPC : MonoBehaviour
{
    public bool isMeetPlayer;
    public bool isTalk;
    private List<string> sentencese;

    private void Start()
    {
        FirstDialogue();
        enemyController =  FindObjectOfType<EnemyController>(); 
    }
    EnemyController enemyController;
    public void FirstDialogue()
    {
        TextAsset npcTalk = Managers.Resource.Load<TextAsset>($"Data/NpcTalk");

        if (npcTalk != null)
        {
            Dialogue talkData = JsonUtility.FromJson<Dialogue>(npcTalk.text);
            sentencese = new List<string>();

            foreach (var entry in talkData.Firstdialogues)
            {
                sentencese.Add($"{entry.dialogue}"); // 이름과 대화를 저장
            }

        }
        else
        {
            Debug.LogWarning("NpcTalk JSON 파일을 찾을 수 없습니다!");
        }
    }
    public void SecondDialogue()
    {
        sentencese.Clear();

        TextAsset npcTalk = Managers.Resource.Load<TextAsset>($"Data/NpcTalk");

        if (npcTalk != null)
        {
            Dialogue talkData = JsonUtility.FromJson<Dialogue>(npcTalk.text);

            foreach (var entry in talkData.Seconddialogues) // Seconddialogues로 전환
            {
                sentencese.Add($"{entry.dialogue}");
            }

            DialogueManager.instance.OnDialogue(sentencese.ToArray());

            StartCoroutine(WaitForDialogueEnd()); //2번째 대화 종료 코루틴 호출
        }
        else
        {
            Debug.LogWarning("NpcTalk JSON 파일을 찾을 수 없습니다!");
        }
    }

    private IEnumerator WaitForDialogueEnd()
    {
      
        while (DialogueManager.instance.DialogueGroup.alpha > 0)
        {
            yield return null; // 2번째 대화가 끝날 때까지 대기
        }
        //대화 끝
        Debug.Log("대화 끝");
        GameManager.Instance.CreateEnemyOption();
        AIShootOption(false); //true - ai 공격 시작

        
    }
    public void AIShootOption(bool isTalk)
    {
        if (isTalk == false)
        {
            Debug.Log("1");
            this.gameObject.GetComponent<AIController>().StartAIShoot();
        }
        else if (isTalk == true)
        {
            Debug.Log("ㅁ");
            this.gameObject.GetComponent<AIController>().StopAIShoot();
        }



    }
    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.E) && isTalk == false)
        {
            if (isMeetPlayer)
            { 
                if (DialogueManager.instance.DialogueGroup.alpha == 0 && enemyController.isTutorialEnemyDie == false)
                {
                    isTalk = true;
                    AIShootOption(isTalk); // 1번째 대화 시작 후 종료시 isTalk = false;
                    DialogueManager.instance.OnDialogue(sentencese.ToArray());
                }
                else if (enemyController.isTutorialEnemyDie == true) //튜토리얼 적 죽었을 때
                {
                    isTalk = true;
                    SecondDialogue(); //2번째 대화 시작 후 종료시 isTalk = false;
                    AIShootOption(isTalk);
                }
                StartCoroutine(DotCoroutine());
            }
        }
      
    }
    public TextMeshPro tmpText; // TMP TextMeshPro 컴포넌트
    private int dotCount = 0; // 현재 마침표 개수
    private bool increasing = true; // 마침표 개수가 증가 중인지 확인


    IEnumerator DotCoroutine()
    {
        if(enemyController.isTutorialEnemyDie == false)
        {
            yield return null;
        }
        while (true) // 무한 루프
        {
            // 마침표 추가
            if(isTalk == false )
            {
                dotCount = 0;
                tmpText.text = new string('.', dotCount);
                yield break;
            }
            if (increasing)
            {
                // 마침표 개수 증가
                dotCount++;
                if (dotCount >= 5) // 5개가 되면 감소 모드로 전환
                {
                    increasing = false;
                }
            }
            else
            {
                // 마침표 개수 감소
                dotCount--;
                if (dotCount <= 0) // 0개가 되면 증가 모드로 전환
                {
                    increasing = true;
                }
            }

            // TMP 텍스트 업데이트
            if(dotCount > 0 )
            {
                tmpText.text = new string('.', dotCount);
            }
           
            yield return new WaitForSeconds(0.5f); // 0.5초 대기
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isMeetPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isMeetPlayer = false;
        }
    }
}
