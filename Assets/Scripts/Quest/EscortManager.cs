using UnityEngine;
using UnityEngine.AI;

// Dùng trong nhiệm vụ hộ tống
public class EscortManager : MonoBehaviour
{
    [Header("Refs")]
    public Transform standPosition;
    public PlayerManager target;
    public NPC npc;

    NavMeshAgent nav;
    Animator anim;

    // Dùng khi hoàn thành nhiệm vụ rồi ==> NPC sẽ đi tới địa điểm chỉ định để đứng ở đấy
    public bool isMovingToStandPosition = false;

    private void Awake() {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        npc = GetComponent<NPC>();
    }

    private void Start() {
        nav.enabled = false;
    }

    private void Update() {
        // Chạy/dừng animation dựa theo nav mesh agent (nếu đang đi ==> chạy animation và ngược lại) 
        anim.SetBool("isMoving", nav.velocity.magnitude > 0.01f);

        // Chạy theo target (player)
        if(target != null){
            nav.SetDestination(target.transform.position);
        }

        // Chạy tới địa điểm chỉ định
        if(isMovingToStandPosition){
            nav.enabled = true;
            nav.SetDestination(standPosition.position);

            // Kiểm tra khoảng cách giữa npc và địa điểm chỉ định xem gần nhau nhất chưa? (khó có thể = nhau đc)
            // Nếu sát nhau thì quay NPC ngược lại (fix trường hợp hướng/nhìn vào tường)
            if(Vector3.Distance(standPosition.position, transform.position) <= 0.05f){
                // Dừng chạy
                nav.enabled = false;
                isMovingToStandPosition = false;

                // Cập nhật rotation
                transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x,
                    0,
                    transform.eulerAngles.z
                );
            }
        }
    }

    // Bắt đầu hộ tống (NPC chạy theo player)
    public void StartEscort(PlayerManager _target){
        nav.enabled = true;
        npc.isEscorting = true;
        target = _target;
    }

    // Hoàn thành hộ tống
    public void CompleteEscort(){
        EventManager.EscortCompleted(npc);

        nav.enabled = false;
        npc.isEscorting = false;
        target = null;
    }

    // Đi tới địa điểm đứng khi nhận thưởng từ NPC xog
    public void MoveToStandPoint(){
        isMovingToStandPosition = true;
    }
}
