using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueInteractuableBase : MonoBehaviour, InterfazInteractuable
{
    [Header("Dialogo")]
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image portraitImage;

    [Header("Comportamiento")]
    [SerializeField] protected bool autoOpenOnPlayerEnter;
    [SerializeField] protected bool closeOnPlayerExit;
    [SerializeField] protected bool hidePortrait;
    [SerializeField] protected bool hideName;
    [SerializeField] protected string displayNameOverride = "";

    protected int dialogueIndex;
    protected bool isTyping;
    protected bool isDialogueActive;

    protected virtual void Awake()
    {
        ResolverReferenciasUI();
    }

    protected virtual void Reset()
    {
    }

    public virtual bool PuedeInteractuar()
    {
        return !isDialogueActive;
    }

    public virtual void Interactuar()
    {
        if (dialogueData == null || (Pausar.estaPausado && !isDialogueActive))
        {
            return;
        }

        ResolverReferenciasUI();

        if (dialoguePanel == null || dialogueText == null)
        {
            Debug.LogWarning($"No se encontraron referencias de dialogo para {name}.");
            return;
        }

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (autoOpenOnPlayerEnter && collision.CompareTag("Player") && !isDialogueActive)
        {
            Interactuar();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (closeOnPlayerExit && collision.CompareTag("Player") && isDialogueActive)
        {
            EndDialogue();
        }
    }

    protected virtual void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        ConfigurarCabecera();

        dialoguePanel.SetActive(true);
        Pausar.PonerPausa(true);

        StartCoroutine(TypeLine());
    }

    protected virtual void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    protected virtual IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLine.Length > dialogueIndex && dialogueData.autoProgressLine[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public virtual void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        isTyping = false;

        if (dialogueText != null)
        {
            dialogueText.SetText("");
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        Pausar.PonerPausa(false);
    }

    protected virtual void ConfigurarCabecera()
    {
        if (nameText != null)
        {
            bool mostrarNombre = !hideName;
            nameText.gameObject.SetActive(mostrarNombre);
            if (mostrarNombre)
            {
                nameText.SetText(ObtenerNombreMostrado());
            }
        }

        if (portraitImage != null)
        {
            Sprite portrait = hidePortrait ? null : dialogueData.npcPortrait;
            portraitImage.gameObject.SetActive(portrait != null);
            if (portrait != null)
            {
                portraitImage.sprite = portrait;
            }
        }
    }

    protected virtual string ObtenerNombreMostrado()
    {
        if (!string.IsNullOrWhiteSpace(displayNameOverride))
        {
            return displayNameOverride;
        }

        return dialogueData.npcName;
    }

    protected void ResolverReferenciasUI()
    {
        if (dialoguePanel != null && dialogueText != null && nameText != null && portraitImage != null)
        {
            return;
        }

        DialogueInteractuableBase[] interactuables = FindObjectsByType<DialogueInteractuableBase>(FindObjectsSortMode.None);

        foreach (DialogueInteractuableBase interactuable in interactuables)
        {
            if (interactuable == this)
            {
                continue;
            }

            if (interactuable.dialoguePanel == null || interactuable.dialogueText == null ||
                interactuable.nameText == null || interactuable.portraitImage == null)
            {
                continue;
            }

            dialoguePanel = interactuable.dialoguePanel;
            dialogueText = interactuable.dialogueText;
            nameText = interactuable.nameText;
            portraitImage = interactuable.portraitImage;
            return;
        }
    }
}
