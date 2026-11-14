using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float maxMoveSpeed;
    public float rotationSpeed;
    public bool isPaused;
    public PlayerSkills skills;
    public int currentAttack => skills.CurrentAttack;
    public CinemachineImpulseSource impulseSource;
    public PlayerCustomizeBody customizeBody;
    CharacterController controller;
    Animator animator;
    Vector3 inputDirection;
    private bool isJumping = false;
    private bool isSpeedBoosted = false;
    private Coroutine speedBoostCoroutine;
    private float appliedSpeedBonus = 0f; // quanto o buff realmente somou (pode ser menor que amount por causa do maxMoveSpeed)
    public static Player Instance;
    public PlayerHealth playerHealth;
    public PlayerHUD playerHUD;
    public PlayerMana playerMana;
    public int coinScore;
    public Sprite speedBuffIcon;
    public EquipmentManager equipmentManager;
    public int baseBodyIndex = 0;

    [Header("Targeting")]
    public float attackRange = 2f;
    private bool isMovingToTarget = false;
    private System.Action pendingAttackAction = null;

    [Header("Gravidade")]
    public float gravity = -9.81f;
    private Vector3 velocity;

    public Animator Animator { get => animator; set => animator = value; }
    public bool IsMovingToTarget => isMovingToTarget;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // duplicata: não continua inicializando
        }

        //pausando o jogo na cena de customização
        string nomeCena = SceneManager.GetActiveScene().name;
        if (nomeCena != "customization")
        {

        }
        else
        {
            isPaused = true;
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        Player.Instance.playerHUD.UpdateCoinText();
        BodyManager.Instance.EquipSuit(baseBodyIndex); // garante que o traje base da customização seja equipado
        skills = GetComponent<PlayerSkills>();
    }

    // Update is called once per frame
    void Update()
    {
        // se o InputManager não estiver pronto, não processa nada
        if (InputManager.Instance == null) return;

        // inventário
        if (Input.GetKeyDown(InputManager.Instance.openInventory))
        {
            playerHUD.ToggleInventory();
        }

        // Status do personagem
        if (Input.GetKeyDown(InputManager.Instance.openStatus))
        {
            playerHUD.ToggleStatus();
        }

        // fechar menus com o botão de fechar e cancelar seleção de item
        if (Input.GetKeyDown(InputManager.Instance.closeMenu))
        {
            InventorySlot.CancelSelection();
            playerHUD.CloseInventory();
            playerHUD.CloseStatus();
            playerHUD.CloseOptions();
            playerHUD.CloseSounds();
            playerHUD.CloseKeybinds();
        }

        // poção de vida, mana e velocidade
        if (Input.GetKeyDown(InputManager.Instance.useHealthPotion) && Inventory.Instance.HasPotion(ItemData.ItemEffects.health))
        {
            Inventory.Instance.UsePotion(ItemData.ItemEffects.health);
            PlayerSoundManager.Instance.PlayUseBottleSfx();
        }
        if (Input.GetKeyDown(InputManager.Instance.useManaPotion) && Inventory.Instance.HasPotion(ItemData.ItemEffects.mana))
        {
            Inventory.Instance.UsePotion(ItemData.ItemEffects.mana);
            PlayerSoundManager.Instance.PlayUseBottleSfx();
        }
        if (Input.GetKeyDown(InputManager.Instance.useSpeedPotion) && Inventory.Instance.HasPotion(ItemData.ItemEffects.speed))
        {
            Inventory.Instance.UsePotion(ItemData.ItemEffects.speed);
            PlayerSoundManager.Instance.PlayUseBottleSfx();
        }

        // apply pause state first
        Paused();

        // when paused, stop processing input/movement/attacks
        if (isPaused)
        {
            // ensure animator shows idle
            Animator.SetFloat("Speed", 0f);
            return;
        }

        if (Input.GetKeyDown(InputManager.Instance.jump) && !isJumping && !skills.IsAttacking)
        {
            Jump();
        }

        if (isMovingToTarget)
        {
            MoveToTarget();
        }
        else
        {
            Move();
        }

        // aplica gravidade sempre, exceto quando pausado
        if (!isPaused)
        {
            ApplyGravity();
        }
       
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // pequena força pra manter grudado no chão
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // mover o personagem
    void Move()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // captura o input do teclado
        if (Input.GetKey(InputManager.Instance.moveRight)) horizontal = 1f;
        if (Input.GetKey(InputManager.Instance.moveLeft)) horizontal = -1f;
        if (Input.GetKey(InputManager.Instance.moveUp)) vertical = 1f;
        if (Input.GetKey(InputManager.Instance.moveDown)) vertical = -1f;

        // pega a direção relativa à câmera
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // ignora o eixo Y da câmera (para não subir/descer)
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        inputDirection = (camForward * vertical + camRight * horizontal).normalized;

        if (inputDirection != Vector3.zero && !skills.IsAttacking)
        {
            // calculo para onde o personagem deve olhar
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);

            // executa a acao de rotacionar o personagem de maneira suave
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // movendo o personagem
            controller.Move(inputDirection * moveSpeed * Time.deltaTime);
        }

        // usa a magnitude combinada para a animação de corrida
        float speedValue = inputDirection.magnitude;
        Animator.SetFloat("Speed", speedValue);
    }

    public void SpeedBoost(int amount)
    {
        if (isSpeedBoosted)
        {
            // já está buffado, só renova o tempo (não soma de novo)
            StopCoroutine(speedBoostCoroutine);
            speedBoostCoroutine = StartCoroutine(SpeedBoostDurationCoroutine());
            BuffManager.Instance.AddBuff("speed", speedBuffIcon, 5f);
            Debug.Log("Speed buff renovado, moveSpeed atual: " + moveSpeed);
            return;
        }

        float baseSpeed = moveSpeed;
        moveSpeed = Mathf.Min(moveSpeed + amount, maxMoveSpeed);
        appliedSpeedBonus = moveSpeed - baseSpeed;

        isSpeedBoosted = true;
        BuffManager.Instance.AddBuff("speed", speedBuffIcon, 5f);
        Debug.Log("Player Speed increased: " + moveSpeed);
        speedBoostCoroutine = StartCoroutine(SpeedBoostDurationCoroutine());
    }

    IEnumerator SpeedBoostDurationCoroutine()
    {
        yield return new WaitForSeconds(5f);
        // remove só o que foi somado de fato, para não ficar mais lento que a velocidade original
        moveSpeed -= appliedSpeedBonus;
        appliedSpeedBonus = 0f;
        Debug.Log("Player Speed boost ended: " + moveSpeed);
        isSpeedBoosted = false;
    }

    public void Jump()
    {
        if (isJumping || skills.IsAttacking || isPaused) return;
        isJumping = true;
        Animator.SetTrigger("Jump");
        StartCoroutine(JumpCollectRoutine());
    }

    IEnumerator JumpCollectRoutine()
    {
        float elapsed = 0f;
        float duration = 0.7f; // duração da animação

        while (elapsed < duration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up * 1.5f, 0.8f);
            foreach (Collider hit in hits)
            {
                hit.GetComponent<Coin>()?.Collect();
                hit.GetComponent<BottleHealth>()?.Collect();
                hit.GetComponent<BottleMana>()?.Collect();
                hit.GetComponent<BottleSpeed>()?.Collect();
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        isJumping = false;
    }

    void Paused()
    {
        // Do not change Time.timeScale here — that pauses the whole scene.
        // Instead, disable the CharacterController so only the player stops moving.
        if (controller != null)
        {
            controller.enabled = !isPaused;
        }
    }

    public void ShakeCamera()
    {
        impulseSource.GenerateImpulse();
    }

    void MoveToTarget()
    {
        // se o player tentar se mover manualmente, cancela o auto-attack
        bool manualInput = Input.GetKey(InputManager.Instance.moveUp) ||
                            Input.GetKey(InputManager.Instance.moveDown) ||
                            Input.GetKey(InputManager.Instance.moveLeft) ||
                            Input.GetKey(InputManager.Instance.moveRight);

        if (manualInput)
        {
            isMovingToTarget = false;
            pendingAttackAction = null;
            return;
        }

        var target = TargetingSystem.Instance.CurrentTarget;

        if (target == null)
        {
            isMovingToTarget = false;
            pendingAttackAction = null;
            return;
        }

        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;
        float effectiveRange = attackRange + 0.3f;

        if (distance <= effectiveRange)
        {
            // chegou perto — para, encara o alvo e dispara o ataque
            isMovingToTarget = false;
            Animator.SetFloat("Speed", 0f);

            FaceTarget(target.transform.position);

            pendingAttackAction?.Invoke();
            pendingAttackAction = null;
            return;
        }

        direction.Normalize();
        float step = moveSpeed * Time.deltaTime;
        float remaining = distance - attackRange;
        if (step > remaining) step = remaining;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        controller.Move(direction * step);
        Animator.SetFloat("Speed", 1f);
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    // chamado pelos métodos de ataque/skill quando há um alvo travado
    public bool TryAttackWithTarget(System.Action attackAction)
    {
        var target = TargetingSystem.Instance != null ? TargetingSystem.Instance.CurrentTarget : null;

        if (target == null) return false; // sem alvo, segue fluxo normal

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance <= attackRange)
        {
            FaceTarget(target.transform.position);
            attackAction.Invoke();
        }
        else
        {
            isMovingToTarget = true;
            pendingAttackAction = attackAction;
        }

        return true; // tratado pelo sistema de target
    }
}
