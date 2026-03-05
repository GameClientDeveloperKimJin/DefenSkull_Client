using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoolTime : MonoBehaviour
{

    [SerializeField]
    private float archerbasicAtk = 5.0f;
    [SerializeField]
    private float archerbasicAtkSet = 5.0f;

    [SerializeField]
    private float archerCurveAtk = 5.0f;
    [SerializeField]
    private float archerCurveAtkSet = 5.0f;

    [SerializeField]
    private float archerMultipleAtk = 5.0f;
    [SerializeField]
    private float archerMultipleAtkSet = 5.0f;


    PlayerController playerController;
    Define.ArrowType arrowType;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {

    }

    public Image baseAttackImage;
    public Text baseAttackText;

    public Image curveAttackImage;
    public Text curveAttackText;

    public Image comboAttackImage;
    public Text comboAttackText;
    public void ArcherBasicAtk()
    {
        baseAttackText.gameObject.SetActive(true);
        baseAttackText.text = ((int)archerbasicAtk).ToString();

        archerbasicAtk -= Time.deltaTime;
        baseAttackImage.fillAmount = archerbasicAtk / archerbasicAtkSet;

        if (archerbasicAtk < 0)
        {
            archerbasicAtk = archerbasicAtkSet;

            playerController.isShoot = false;
            playerController.atkCount = 0;
            playerController.isMove = true;

            baseAttackText.gameObject.SetActive(false);
            baseAttackImage.fillAmount = 1.0f;
        }
    }

    public void ArcherCurveAtk()
    {
        curveAttackText.gameObject.SetActive(true);
        curveAttackText.text = ((int)archerCurveAtk).ToString();

        archerCurveAtk -= Time.deltaTime;
        curveAttackImage.fillAmount = archerCurveAtk / archerCurveAtkSet;

        if (archerCurveAtk < 0)
        {
            archerCurveAtk = archerCurveAtkSet;

            playerController.isCurveShoot = false;

            curveAttackText.gameObject.SetActive(false);
            curveAttackImage.fillAmount = 1.0f;

        }
    }

    public void ArcherMultipleAtk()
    {
        comboAttackText.gameObject.SetActive(true);
        comboAttackText.text = ((int)archerMultipleAtk).ToString();

        archerMultipleAtk -= Time.deltaTime;
        comboAttackImage.fillAmount = archerMultipleAtk / archerMultipleAtkSet;

        if (archerMultipleAtk < 0)
        {
            archerMultipleAtk = archerMultipleAtkSet;

            playerController.isMultipleShoot = false;

            comboAttackText.gameObject.SetActive(false);
            comboAttackImage.fillAmount = 1.0f;
        }
    }
}