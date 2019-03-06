using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    //Movement
    [Header("Move")]
    [SerializeField] private float SetMovementSpeed;
    [SerializeField] private string HorizontalInputName;
    [SerializeField] private string VerticalInputName;
    [SerializeField] private KeyCode WalkKey;
    [SerializeField] private KeyCode WalkHoldKey;
    [SerializeField] private KeyCode CrouchKey;
    [SerializeField] private KeyCode CrouchHoldKey;
    private float MovementSpeed;
    private bool Walking;
    private bool Crouching;
    //Jump
    [Header("Jump")]
    [SerializeField] private AnimationCurve JumpFallOff;
    [SerializeField] private float JumpMultiplier;
    [SerializeField] private KeyCode JumpKey;
    private bool IsJumping;

    //Shooting
    [Header("Shooting")]
    [SerializeField] private KeyCode FireGun;
    [SerializeField] public int Ammo;
    [SerializeField] public int ReserveAmmo;
    [SerializeField] GameObject ShootingPoint;
    [SerializeField] private Camera PlayerCamera;
    public bool IsShooting;
    private bool CanShoot;
    public bool HasGun;
    public float TimeBetweenShots;
    private float GunRefresh;
    public AudioClip GunShot;
    public AudioClip GunEmpty;
    private AudioSource MyAudio;
    [SerializeField] private float Recoil;
    [SerializeField] private float RecoilIntensity;
    public float RecoilPower;


    [Header("Affects")]
    [SerializeField] private GameObject Dust;
    public Animator GunAnimator;
    [SerializeField] GameObject MuzzleFlareLight;
    public GameObject MuzzleFlare;
    public GameObject MuzzleFlareLong;

    //Cursors
    [SerializeField] Image Crossair;
    [SerializeField] Sprite Crossair1;
    [SerializeField] Sprite Crossair2;
    [SerializeField] Sprite Crossair3;
    [SerializeField] Sprite Crossair4;
    [SerializeField] Sprite Interactable;
    [SerializeField] Sprite Default;
    private bool LookingAtClickable;

    [Header("UI")]
    [SerializeField] Text UIAmmo;

    //Bulletholes
    [Header("ImpactDecal")]
    [SerializeField] GameObject WoodImpact;

    [Header("Animation")]
    [SerializeField] Animator FPAnimator;



    private CharacterController CharController;

    // Start is called before the first frame update
    void Awake()
    {
        RecoilPower = .25f;
        MuzzleFlareLight.SetActive(false);
        Recoil = 0;
        Walking = false;
        IsShooting = false;
        CanShoot = true;
        HasGun = true;
        Ammo = 24;
        TimeBetweenShots = 0.108f;
        GunRefresh = 0;
        MovementSpeed = SetMovementSpeed;
        CharController = GetComponent<CharacterController>();
        MyAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        Shoot();
        PlayerMovement();
        lookForInteractable();
        UpdateUI();
        Reload();
    }

    private void PlayerMovement()
    {
        float VertInput = Input.GetAxisRaw(VerticalInputName) * MovementSpeed;
        float HorizontalInput = Input.GetAxisRaw(HorizontalInputName) * MovementSpeed;

        Vector3 ForwardMovement = transform.forward * VertInput;
        Vector3 RightMovment = transform.right * HorizontalInput;

        CharController.SimpleMove(ForwardMovement + RightMovment);

        JumpInput();
        WalkToggle();
        CrouchToggle();
    }

    private void JumpInput()
    {
        if(Input.GetKeyDown(JumpKey) && !IsJumping)
        {
            IsJumping = true;
            StartCoroutine(JumpEvent());

        }
    }

    private IEnumerator JumpEvent()
    {
        CharController.slopeLimit = 90.0f;
        float TimeInAir = 0.0f;

        do
        {
            float jumpForce = JumpFallOff.Evaluate(TimeInAir);
            CharController.Move(Vector3.up * jumpForce * JumpMultiplier * Time.deltaTime);
            TimeInAir += Time.deltaTime;
            yield return null;
        } while (!CharController.isGrounded && CharController.collisionFlags != CollisionFlags.Above);


        CharController.slopeLimit = 45.0f;
        IsJumping = false;
    }

    private void CrouchToggle()
    {
        if (Input.GetKeyDown(CrouchKey))
        {
            if (!Crouching)
            {
                Crouching = true;
                CharController.height = 1.2f;
            }
            else
            {
                Crouching = false;
                CharController.height = 2f;
            }
        }
        if (Input.GetKeyDown(CrouchHoldKey))
        {
            if (!Crouching)
            {
                Crouching = true;
                CharController.height = 1.2f;
            }
            else
            {
                Crouching = false;
                CharController.height = 2f;
            }
        }
        if (Input.GetKeyUp(CrouchHoldKey))
        {
            if (!Crouching)
            {
                Crouching = true;
                CharController.height = 1.2f;
            }
            else
            {
                Crouching = false;
                CharController.height = 2f;
            }
        }
    }

    private void WalkToggle()
    {
        if (Input.GetKeyDown(WalkKey) && !Input.GetKeyDown(WalkHoldKey))
        {
            if (!Walking)
            {
                Walking = true;
                MovementSpeed = SetMovementSpeed*.60f;
            }
            else
            {
                Walking = false;
                MovementSpeed = SetMovementSpeed;
            }
        }
        if (Input.GetKey(WalkHoldKey) && !Walking)
        {
            Walking = true;
            MovementSpeed = SetMovementSpeed * .60f;
        }
        if (Input.GetKeyUp(WalkHoldKey))
        {
            Walking = false;
            MovementSpeed = SetMovementSpeed;
        }
    }

    void Shoot()
    {
        if (Recoil > 0)
        {
            Recoil -= 1 * Time.deltaTime;
        }
        if (GunRefresh > 0)
        {
            GunRefresh -= 1 * Time.deltaTime;
        }
        if (Input.GetKeyDown(FireGun))
        {
            GunAnimator.SetTrigger("PullTrigger");
            if (Ammo <= 0)
            {
                MyAudio.PlayOneShot(GunEmpty, 1);
            }
        }
        if (Input.GetKey(FireGun) && CanShoot && GunRefresh <= 0 && Ammo > 0)
        {
            MuzzleFlareLight.SetActive(true);
            GameObject Flash = Instantiate(MuzzleFlare, ShootingPoint.transform.position, PlayerCamera.transform.rotation);
            GameObject FlashLong = Instantiate(MuzzleFlareLong, ShootingPoint.transform.position, PlayerCamera.transform.rotation);
            float FlashRandomScale = Random.Range(.1f, .2f);
            float FlashRandomScaleLong = Random.Range(.8f, 1.2f);
            FlashLong.transform.localScale = new Vector3(FlashRandomScaleLong, FlashRandomScaleLong, FlashRandomScaleLong);
            FlashLong.transform.Rotate(0, 90, 0);
            Flash.transform.localScale = new Vector3(FlashRandomScale, FlashRandomScale, FlashRandomScale);
            Flash.transform.Rotate(Random.Range(0, 360), 90, 0);
            GunAnimator.SetTrigger("Fire");
            Ammo--;
            MyAudio.PlayOneShot(GunShot, 1);
            PlayerCamera.transform.Rotate(new Vector3(-(RecoilPower*4),0,0));
            GunRefresh = TimeBetweenShots;
            RaycastHit Hit;
            Vector3 ShotOffSet = new Vector3(Random.Range(-RecoilIntensity, RecoilIntensity) * Recoil, Random.Range(-RecoilIntensity, RecoilIntensity) * Recoil, 0);
            if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward + ShotOffSet, out Hit))
            {
                Hit.transform.gameObject.SendMessage("HitByRay", SendMessageOptions.DontRequireReceiver);
                var Bullethole = Instantiate(WoodImpact, Hit.point, Quaternion.FromToRotation(Vector3.up, Hit.normal));
                Bullethole.transform.SetParent(Hit.transform);
                if (Hit.collider.tag == "Enemy")
                {
                    Hit.collider.GetComponent<Monster>().HP -= 1;
                }
                if (Hit.rigidbody != null)
                {
                    Debug.Log("Hit Rigid");
                    Hit.rigidbody.AddForce(PlayerCamera.transform.forward * 3, ForceMode.Impulse);
                }
                Instantiate(Dust, Hit.point, PlayerCamera.transform.rotation);
            }
            Recoil += RecoilPower;
        }
        if (Input.GetKeyUp(FireGun) || Ammo <= 0)
        {
            GunAnimator.SetTrigger("EndFire");
        }
        if (Recoil > 1) { Recoil = 1; }

        if (Recoil > .75f) { Crossair.GetComponent<Image>().overrideSprite = Crossair4; }
        else
        {
            if (Recoil > .40f) { Crossair.GetComponent<Image>().overrideSprite = Crossair3; }
            else
            {
                if (Recoil > .15f) { Crossair.GetComponent<Image>().overrideSprite = Crossair2; }
                else
                { Crossair.GetComponent<Image>().overrideSprite = Crossair1; }
            }
        }
    }

    void lookForInteractable()
    {
        RaycastHit Press;
        if(Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward * .1f, out Press))
        {
            if (Press.collider.tag == "Interactable")
            {
                LookingAtClickable = true;
                Crossair.GetComponent<Image>().overrideSprite = Interactable;
                if (Input.GetButtonDown("Interact"))
                {
                    Press.collider.gameObject.GetComponent<Pressable>().Toggle();
                }
            }
            else
            {
                LookingAtClickable = false;
            }
        }

    }

    void UpdateUI()
    {
        UIAmmo.text = Ammo.ToString() + "/" + ReserveAmmo.ToString();
    }

    void Reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            CanShoot = false;
            StartCoroutine(ReloadRoutine());
            IEnumerator ReloadRoutine()
            {
                if (ReserveAmmo > 0)
                {
                    if (ReserveAmmo > 24)
                    {
                        FPAnimator.SetTrigger("Reload");
                        yield return new WaitForSeconds(FPAnimator.runtimeAnimatorController.animationClips[1].length);
                        ReserveAmmo -= (24-Ammo);
                        Ammo = 24;
                    }
                    else
                    {
                        FPAnimator.SetTrigger("Reload");
                        yield return new WaitForSeconds(FPAnimator.runtimeAnimatorController.animationClips[1].length);
                        Ammo = ReserveAmmo;
                        ReserveAmmo = 0;
                    }
                }
                CanShoot = true;
                yield return null;
            }
        }
    }
}
