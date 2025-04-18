using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScObjects;
using UI;
using Weapon;

namespace Player
{
    public class PlayerScript : MonoBehaviour, ICanTakeDamage
    {
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private WeaponScript weaponScript;
        [SerializeField] private WeaponbarScript weaponBar;
        [SerializeField] private HPbarScript HPBar;
        [SerializeField] private MachineScObject machineSc; 
        //��� ������, � ������� ���� SerializeField ���� ������, ��� ������ ����� ���������� ����� ��������� ������
        [SerializeField] private WeaponScObject weaponSc;
        //��� ������, � ������� ���� SerializeField ���� ������, ��� ������ ����� ���������� ����� ��������� ������

        public Rigidbody2D PlayerRigidbody2D { get; private set; }
        private float currentHp;

        public bool TakeDamage(float value)
        {
            currentHp -= value;
            HPBar.SetBarProgress(currentHp / machineSc.HitPoints);
            HPBar.SetText(currentHp.ToString());
            if (currentHp <= 0)
            {
                currentHp -= value;
                if (currentHp <= 0) 
                    Death();

                print(currentHp);
                return true;
            }
            
            return true;
        }

        private bool Fire()
        {
            var current = weaponScript.Fire();
            if (current > 0) 
                UpdateWeaponBar(current.ToString(), Color.white);
            
            return true;
        }

        private void UpdateWeaponBar(string value, Color color) => weaponBar.UpdateNumber(value, color);

        private void Death() => SceneManager.LoadScene("Main menu");

        private void Start()
        {
            weaponScript.SetScObject(weaponSc);
            weaponBar.UpdateName(weaponSc.WeaponName);
            UpdateWeaponBar(weaponSc.MagSize.ToString(), Color.white);
            currentHp = machineSc.HitPoints;
            PlayerRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            playerMovement.MovePlayer(playerInput.GetMovementInput());
            playerMovement.RotatePlayer(playerInput.GetMousePosition());
            if (playerInput.GetFireInput()) 
                Fire();
            
            if (weaponScript.Reloading)
            {
                UpdateWeaponBar((math.round(weaponScript.ReloadTimer * 10) / 10).ToString(), 
                    new Color(0.63f, 0.15f, 0.05f, 1));
                
                if (weaponScript.ReloadTimer - Time.deltaTime <= 0) 
                    UpdateWeaponBar(weaponSc.MagSize.ToString(), Color.white);
            }
        }
    }
}