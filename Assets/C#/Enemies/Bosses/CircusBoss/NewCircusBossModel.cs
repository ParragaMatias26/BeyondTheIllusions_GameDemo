using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class NewCircusBossModel : Boss
{
    [SerializeField] GameObject bossTrigger;

    [Header("Phase One Values")]
    [SerializeField] private float platformJumpSpeed = 15f;

    [SerializeField] private PlatformManager myPlatformManager;
    [SerializeField] private AudienceManager myAudienceManager;
    [SerializeField] private CarSpawnManager[] myCarSpawners;

    [Header("Juggle Attack Values")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private Vector2 juggleOffset;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject mushroomPrefab;
    [SerializeField] private float throwVelocity = 5f;
    [SerializeField] private float juggleRadius = 1.2f;
    [SerializeField] private float juggleSpeed = 5f;
    [SerializeField] private float juggleDuration = 3f;
    [SerializeField] private int proyectileDamage = 2;

    [Header("Spiral Values")]
    [SerializeField] private GameObject spiralColliders;
    [SerializeField] private float spiralMovSpeed;
    [SerializeField] private float spiralDuration;

    [Header("Dash Values")]
    [SerializeField] private int dashCount = 3;
    [SerializeField] private float dashDuration = 8f;
    [SerializeField] private float betweenDashesDelay = 1f;
    [SerializeField] private float delayBeforeDash = .5f;
    [SerializeField] private GameObject spotLightPrefab;

    [Header("View Settings")]
    [SerializeField] string introPhaseIntName;
    [SerializeField] string juggleTriggerName;
    [SerializeField] string juggleEndBoolName;
    [SerializeField] string throwTriggerName;
    [SerializeField] string spinerAttackBoolName;
    [SerializeField] string dashAttackBoolName;
    [SerializeField] string hurtTriggerName;
    [SerializeField] string jumpTriggerName;
    [SerializeField] string screamBoolName;
    [SerializeField] string tiredBoolName;

    [Header("Boss Intro Values")]
    [SerializeField] private Transform introStartPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private float descentDelay = 1.5f;
    [SerializeField] private float descentDuration;
    [SerializeField] private float screamDuration = 2f;

    [Header("Player Kill Settings")]
    [SerializeField] private GameObject mainLight;
    [SerializeField] private GameObject playerLights;
    [SerializeField] private GameObject damageVFX;
    [SerializeField] private Transform centerArena;

    [Header("On Death Values")]
    [SerializeField] GameObject dashBootsPrefab;
    [SerializeField] GameObject exitObject;
    [SerializeField] GameObject deathParticlesVFX;

    [SerializeField] private CapsuleCollider2D myCol;
    private Vector2 dashDirection;

    private readonly float[] juggleChances = { .1f, .5f, .7f, 1f };
    private int failedJuggleAttempts = 0;

    private Coroutine currentMovement;
    private List<GameObject> currentProyectiles = new List<GameObject>();

    private void Start()
    {
        Initialize();
        myPlatformManager.InitializePlatforms();

        myCol = GetComponent<CapsuleCollider2D>();

        health.CanTakeDamage = false;
        transform.position = introStartPos.position;
        OnBossStart += () =>
        {
            foreach (var spawner in myCarSpawners)
                spawner.StartSpawnLoop();

            myAudienceManager.StartLoop();
        };
        OnBossStop += () =>
        {
            foreach (var spawner in myCarSpawners)
                spawner.StopSpawnLoop();

            myAudienceManager.StopLoop();
        };
        OnBossReset += () =>
        {
            ResetAllAnims();
            DestroyAllProyectiles();
            myPlatformManager.ResetPlatforms();

            spiralColliders.SetActive(false);
            SetBossPhase(1);

            bossTrigger.SetActive(true);
            transform.position = myPlatformManager.currentPlatform.BossPosition;

            GameManager.Instance.canActiveVFX = true;
        };
        myPlatformManager.OnPlatformDestroy += () =>
        {

            canAttack = false;
            StopAttack();

            view.ResetTriggers(juggleTriggerName);
            view.SetAnimatorsBool(juggleEndBoolName, false);

            DestroyAllProyectiles();
            StartCoroutine(ScreamRoutine());
        };
        myPlatformManager.OnLastPlatformDestroy += () =>
        {
            StopAttack();
            SetBossPhase(2);
        };
        PlayerModel.OnPlayerDie += () =>
        {
            CutsceneManager.Instance.CancelZoom();

            StopAllCoroutines();
            SetBossPhase(1);

            GameManager.Instance.ToggleVFX(true);
            DestroyAllProyectiles();

            if (Vector2.Distance(transform.position, target.position) < 20f) StartCoroutine(KillPlayer_Execute());

            spiralColliders.SetActive(false);
            ResetAllAnims();
        };
        CheckpointSystem.Instance.OnRespawn += () =>
        {
            if (isBossActive)
            {
                ResetBoss();
                health.CanTakeDamage = false;

                view.TriggerAnimation("PlayerDeadEnd");
                StopBoss();
            }
        };
        health.OnDamageTake += (_, _) =>
        {
            damageBurst.TriggerEffect();
            spriteFlash.StartFlash();
        };
        health.OnDeath += (_, _) =>
        {
            StopAllCoroutines();
            StopBoss();

            view.SetAnimatorsBool(tiredBoolName, false);
            view.SetAnimatorsBool(screamBoolName, true);
            StartCoroutine(Die_Execute());
        };
        OnBossCancelAttack += () =>
        {
            ResetAllAnims();
        };
        OnAttackCDStart += () =>
        {
            if(currentBossPhase == 2)
                view.SetAnimatorsBool(tiredBoolName, true);
        };
        OnAttackCDEnd += () =>
        {
            if (currentBossPhase == 2)
                view.SetAnimatorsBool(tiredBoolName, false);
        };
    }
    private void Update()
    {
        if (myFSM != null)
            myFSM.ArtificialUpdate();

        if (Vector3.Distance(centerArena.position, target.position) > 30f && isBossActive)
        {
            ResetBoss();
            StopBoss();
        }
    }
    public void SetIntroPhaseIndex(int index) => view.SetAnimatorsInt(introPhaseIntName, index);
    public override IEnumerator BossIntro_Execute()
    {
        SetIntroPhaseIndex(0);
        SetIntroPhaseIndex(1);
        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(descentDelay);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / descentDuration;
            transform.position = Vector3.Lerp(startPos, endPos.position, t);

            yield return null;
        }

        SetIntroPhaseIndex(2);
        transform.position = endPos.position;

        StartBoss();
    }
    public override void SetBossPhase(int index)
    {
        switch (index)
        {
            case 0:
                Debug.Log("Invalid Index");
                break;
            case 1:
                SetPhaseOne();
                currentBossPhase = index;
                break;
            case 2:
                SetPhaseTwo();
                currentBossPhase = index;
                break;
        }
    }
    public override void SelectRandomAttack()
    {
        int randomIndex = Random.Range(0, myAttacks.Count);
        float randomValue = Random.value;

        float currentChance = juggleChances[failedJuggleAttempts];
        Debug.Log($"RandomIndex: {randomIndex}  CurrentChance: {currentChance * 100f}%  RandomValue: {randomValue}");

        if (currentBossPhase == 2)
        {
            if (randomValue <= currentChance)
            {
                Debug.Log("Malabareando!");
                failedJuggleAttempts = 0;

                canAttack = false;
                currentAttack = StartCoroutine(JuggleAttack_Execute());
                return;
            }

            failedJuggleAttempts++;
        }

        if (myAttacks == null || myAttacks.Count == 0)
        {
            Debug.LogWarning("No hay ataques en myAttacks.");
            return;
        }

        if (myAttacks.Count > 1 && lastUsedAttack != null)
        {
            int attempts = 0;
            while (myAttacks[randomIndex] == lastUsedAttack && attempts < 8)
            {
                randomIndex = Random.Range(0, myAttacks.Count);
                attempts++;
            }
        }

        canAttack = false;
        lastUsedAttack = myAttacks[randomIndex];
        currentAttack = StartCoroutine(myAttacks[randomIndex].Execute());

    }
    void SetPhaseOne()
    {
        StopAttack();
        myAttacks.Clear();

        myCol.enabled = false;
        BossAttack PhaseOneJuggle = new BossAttack
        {
            attackName = "PhaseOneJuggle",
            Execute = PhaseOneJuggle_Execute
        };

        myAttacks.Add(PhaseOneJuggle);
    }
    void SetPhaseTwo()
    {
        StopAttack();
        myAttacks.Clear();

        myCol.enabled = true;

        enableAttackCDEvents = true;

        myAudienceManager.StopLoop();

        BossAttack EyeSpiral = new BossAttack
        {
            attackName = "EyeSpiral",
            Execute = EyeSpiral_Execute
        };
        BossAttack BossDash = new BossAttack
        {
            attackName = "BossDash",
            Execute = DashAttack_Execute
        };

        myAttacks.Add(EyeSpiral);
        myAttacks.Add(BossDash);

        health.CanTakeDamage = true;
    }
    public void MoveToPosition(Vector3 endPos, float speed, bool normalizeDir)
    {
        if (currentMovement != null)
            StopCoroutine(currentMovement);

        currentMovement = StartCoroutine(GoToPos_Execute(endPos, speed, normalizeDir));
    }
    IEnumerator GoToPos_Execute(Vector3 endPos, float speed, bool normalizeDir)
    {
        view.TriggerAnimation(jumpTriggerName);

        while (Vector3.Distance(transform.position, endPos) > .5f)
        {
            var dir = (endPos - transform.position);
            if (normalizeDir) dir.Normalize();

            transform.position += dir * (speed * Time.deltaTime);

            yield return null;
        }

        transform.position = endPos;
    }
    IEnumerator PhaseOneJuggle_Execute()
    {
        view.TriggerAnimation(juggleTriggerName);

        yield return new WaitForSeconds(.5f);
        GameObject[] jugglingObjs = new GameObject[3];

        jugglingObjs[0] = Instantiate(ballPrefab, throwPoint.position, Quaternion.identity);
        jugglingObjs[0].GetComponent<Projectile>().doDamageToPlayer = false;
        currentProyectiles.Add(jugglingObjs[0]);

        jugglingObjs[1] = Instantiate(ballPrefab, throwPoint.position, Quaternion.identity);
        jugglingObjs[1].GetComponent<Projectile>().doDamageToPlayer = false;
        currentProyectiles.Add(jugglingObjs[1]);

        jugglingObjs[2] = Instantiate(ballPrefab, throwPoint.position, Quaternion.identity);
        jugglingObjs[2].GetComponent<Projectile>().doDamageToPlayer = false;
        currentProyectiles.Add(jugglingObjs[2]);

        jugglingObjs[0].AddComponent<JuggleOrbit>().Initialize(transform.position + (Vector3)juggleOffset, juggleRadius, juggleSpeed, 0f);
        jugglingObjs[1].AddComponent<JuggleOrbit>().Initialize(transform.position + (Vector3)juggleOffset, juggleRadius, juggleSpeed, 120f * Mathf.Deg2Rad);
        jugglingObjs[2].AddComponent<JuggleOrbit>().Initialize(transform.position + (Vector3)juggleOffset, juggleRadius, juggleSpeed, 240f * Mathf.Deg2Rad);

        yield return new WaitForSeconds(1f);

        if (jugglingObjs[0] != null) jugglingObjs[0].GetComponent<Projectile>().doDamageToPlayer = true;
        LaunchFromOrbit(jugglingObjs[0], target.position, throwVelocity, false, mushroomPrefab);
        view.TriggerAnimation(throwTriggerName);
        yield return new WaitForSeconds(1f);

        if (jugglingObjs[1] != null) jugglingObjs[1].GetComponent<Projectile>().doDamageToPlayer = true;
        LaunchFromOrbit(jugglingObjs[1], target.position, throwVelocity, false, mushroomPrefab);
        view.TriggerAnimation(throwTriggerName);
        yield return new WaitForSeconds(1f);

        if (jugglingObjs[2] != null) jugglingObjs[2].GetComponent<Projectile>().doDamageToPlayer = true;
        LaunchFromOrbit(jugglingObjs[2], target.position, throwVelocity, false, mushroomPrefab);
        view.TriggerAnimation(throwTriggerName);

        yield return new WaitForSeconds(juggleDuration);
        view.SetAnimatorsBool(juggleEndBoolName, true);

        currentProyectiles.Clear();

        currentAttack = null;

        if (currentAttackCD != null)
            StopCoroutine(currentAttackCD);

        currentAttackCD = StartCoroutine(AttackCD(attackCooldown));

        yield return new WaitForSeconds(.5f);
        view.SetAnimatorsBool(juggleEndBoolName, false);
    }
    IEnumerator JuggleAttack_Execute()
    {
        view.TriggerAnimation(juggleTriggerName);

        if (attackInterrupted)
        {
            CleanUpAttack();
            yield break;
        }

        yield return new WaitForSeconds(.5f);
        GameObject[] jugglingObjs = new GameObject[3];

        jugglingObjs[0] = Instantiate(ballPrefab, throwPoint.position, Quaternion.identity);
        jugglingObjs[0].GetComponent<Projectile>().doDamageToPlayer = false;

        jugglingObjs[1] = Instantiate(ballPrefab, throwPoint.position, Quaternion.identity);
        jugglingObjs[1].GetComponent<Projectile>().doDamageToPlayer = false;

        jugglingObjs[2] = Instantiate(mushroomPrefab, throwPoint.position, Quaternion.identity);
        jugglingObjs[2].AddComponent<Projectile>().doDamageToPlayer = false;

        currentProyectiles.AddRange(jugglingObjs);

        jugglingObjs[0].AddComponent<JuggleOrbit>().Initialize(transform.position + (Vector3)juggleOffset, juggleRadius, juggleSpeed, 0f);
        jugglingObjs[1].AddComponent<JuggleOrbit>().Initialize(transform.position + (Vector3)juggleOffset, juggleRadius, juggleSpeed, 120f * Mathf.Deg2Rad);
        jugglingObjs[2].AddComponent<JuggleOrbit>().Initialize(transform.position + (Vector3)juggleOffset, juggleRadius, juggleSpeed, 240f * Mathf.Deg2Rad);

        yield return new WaitForSeconds(1f);

        if (jugglingObjs[0] != null) jugglingObjs[0].GetComponent<Projectile>().doDamageToPlayer = true;
        LaunchFromOrbit(jugglingObjs[0], target.position, throwVelocity, false, mushroomPrefab);
        view.TriggerAnimation(throwTriggerName);
        yield return new WaitForSeconds(1f);

        if (jugglingObjs[1] != null) jugglingObjs[1].GetComponent<Projectile>().doDamageToPlayer = true;
        LaunchFromOrbit(jugglingObjs[1], target.position, throwVelocity, false, mushroomPrefab);
        view.TriggerAnimation(throwTriggerName);
        yield return new WaitForSeconds(1f);

        if (jugglingObjs[2] != null) jugglingObjs[2].GetComponent<Projectile>().doDamageToPlayer = true;
        LaunchFromOrbit(jugglingObjs[2], target.position, throwVelocity, true, mushroomPrefab);
        view.TriggerAnimation(throwTriggerName);

        yield return new WaitForSeconds(juggleDuration);
        view.SetAnimatorsBool(juggleEndBoolName, true);

        currentProyectiles.Clear();

        currentAttack = null;
        if (currentAttackCD != null)
            StopCoroutine(currentAttackCD);

        currentAttackCD = StartCoroutine(AttackCD(attackCooldown));

        yield return new WaitForSeconds(.5f);
        view.SetAnimatorsBool(juggleEndBoolName, false);
    }
    IEnumerator EyeSpiral_Execute()
    {
        yield return new WaitForSeconds(1.5f);

        float minDistance = 1f;
        float maxDistance = 4f;

        Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(minDistance, maxDistance);
        Vector3 finalPos = (Vector2)target.position + offset;
        finalPos.z = target.position.z;

        MoveToPosition(finalPos, spiralMovSpeed, true);

        yield return new WaitForSeconds(1f);
        spiralColliders.SetActive(true);

        view.SetAnimatorsBool(spinerAttackBoolName, true);

        yield return new WaitForSeconds(spiralDuration);
        spiralColliders.SetActive(false);

        view.SetAnimatorsBool(spinerAttackBoolName, false);

        currentAttack = null;
        currentAttackCD = StartCoroutine(AttackCD(attackCooldown));
    }
    IEnumerator DashAttack_Execute()
    {
        yield return new WaitForSeconds(.5f);
        StartDash();
    }
    void StartDash()
    {
        if (currentAttack != null)
            StopCoroutine(currentAttack);

        currentAttack = StartCoroutine(DashRoutine());
    }
    IEnumerator DashRoutine()
    {
        for (int i = 0; i < dashCount; i++)
        {
            Vector3 targetPos = target.position;
            targetPos.z = 0f;

            GameObject spotLight = Instantiate(spotLightPrefab, targetPos, Quaternion.identity);

            spotLight.transform.position = targetPos;
            yield return new WaitForSeconds(delayBeforeDash);

            float t = 0f;

            view.SetAnimatorsBool(dashAttackBoolName, true);
            var dir = (targetPos - transform.position);
            view.SetSpriteDirection(dir.x);

            while (t < dashDuration)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, targetPos, t / dashDuration);
                yield return null;
            }

            transform.position = targetPos;

            Destroy(spotLight);
            view.SetAnimatorsBool(dashAttackBoolName, false);
            yield return new WaitForSeconds(betweenDashesDelay);
        }

        Debug.Log("Ataque Terminado");
        currentAttack = null;
        currentAttackCD = StartCoroutine(AttackCD(attackCooldown));

        #region OldDash
        /*float timer = dashDuration;
        while (timer > 0f)
        {
            transform.Translate(dashDirection.normalized * dashSpeed * Time.deltaTime, Space.World);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, .5f, wallMask);
            if (hit.collider != null)
            {
                var ran = UnityEngine.Random.Range(0, 101);
                if (ran <= 30)
                    dashDirection = (Vector2)target.position - (Vector2)transform.position;
                else
                    dashDirection = Vector3.Reflect(dashDirection, hit.normal);
            }

            view.SetSpriteDirection(dashDirection.x);
            timer -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(.5f);
        currentAttack = null;
        circleCollider.enabled = false;

        view.SetAnimatorsBool(dashAttackBoolName, false);

        currentAttack = null;
        currentAttackCD = StartCoroutine(AttackCD(attackCooldown));*/
        #endregion
        yield return null;
    }
    void DestroyAllProyectiles()
    {
        foreach (GameObject item in currentProyectiles)
            Destroy(item);

        currentProyectiles.Clear();
    }
    IEnumerator ScreamRoutine()
    {
        canAttack = false;
        DestroyAllProyectiles();

        view.TriggerAnimation(hurtTriggerName);
        yield return new WaitForSeconds(1f);

        view.SetAnimatorsBool(screamBoolName, true);
        GameManager.Instance.CameraController.ShakeCamera(screamDuration + 1f, .6f);


        RealityChangeManager.Instance.ToggleCuteWorld(false);
        var halManager = GameManager.Instance.Player._hallucinationManager;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            halManager.HallucinationAmmount = Mathf.Lerp(halManager._hallucinationAmmount, 0f, t);
            yield return null;
        }

        halManager.HallucinationAmmount = 0f;

        yield return new WaitForSeconds(screamDuration);
        view.SetAnimatorsBool(screamBoolName, false);

        yield return new WaitForSeconds(1.5f);
        if (myPlatformManager.currentPlatform != null) MoveToPosition(myPlatformManager.currentPlatform.BossPosition, platformJumpSpeed, true);

        yield return new WaitForSeconds(1f);
        canAttack = true;
    }
    void LaunchFromOrbit(GameObject obj, Vector2 targetPos, float speed, bool isMush, GameObject mushPrefab)
    {
        if (obj == null) return;

        var orbit = obj.GetComponent<JuggleOrbit>();
        if (orbit != null)
            Destroy(orbit);

        Vector2 dir = (targetPos - (Vector2)obj.transform.position).normalized;

        var proj = obj.GetComponent<Projectile>();
        proj.Initialize(dir, targetPos, speed, isMush, mushPrefab, proyectileDamage, damageVFX);
    }
    IEnumerator Die_Execute()
    {
        GameManager.Instance.CameraController.StopAllCoroutines();
        GameManager.Instance.StopGlobalMovement();

        Instantiate(deathParticlesVFX, transform.position, Quaternion.identity, transform);
        GameManager.Instance.CameraController.ShakeCamera(3, .8f);

        var halManager = GameManager.Instance.Player._hallucinationManager;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            halManager.HallucinationAmmount = Mathf.Lerp(halManager._hallucinationAmmount, 0f, t);
            yield return null;
        }

        halManager.HallucinationAmmount = 0f;

        //Temp
        StartCoroutine(GameManager.Instance.EndGame_Execute());
        yield break;

        /*yield return new WaitForSeconds(3f);

        Instantiate(dashBootsPrefab, transform.position, Quaternion.identity);
        exitObject.SetActive(true);

        GameManager.Instance.ResumeGlobalMovement();
        CutsceneManager.Instance.CancelZoom();

        Destroy(gameObject);
        Debug.Log("Logro Obtenido: Derrota a Circus Master");*/

    }
    IEnumerator KillPlayer_Execute()
    {
        mainLight.SetActive(false);
        playerLights.SetActive(false);

        DestroyAllProyectiles();

        yield return new WaitForSeconds(1f);
        transform.position = centerArena.transform.position;
        target.position = centerArena.transform.position;

        view.TriggerAnimation(view._onPlayerKillTriggerName);

        yield return new WaitForSeconds(.3f);
        mainLight.SetActive(true);
        playerLights.SetActive(true);
    }
    void ResetAllAnims()
    {
        view.SetAnimatorsBool(dashAttackBoolName, false);
        view.SetAnimatorsBool(spinerAttackBoolName, false);
        view.SetAnimatorsBool(juggleEndBoolName, false);
        view.SetAnimatorsBool(tiredBoolName, false);
        view.ResetTriggers(juggleTriggerName);
        view.ResetTriggers(hurtTriggerName);
        view.ResetTriggers(jumpTriggerName);
        view.ResetTriggers(screamBoolName);
    }
    private void CleanUpAttack()
    {
        DestroyAllProyectiles();
        currentAttack = null;
        attackInterrupted = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerArena.position, 20f);
    }
}
