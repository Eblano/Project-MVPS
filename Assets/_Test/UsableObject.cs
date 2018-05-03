namespace SealTeam4
{
    public class UsableObject : InteractableObject
    {
        private bool use = false;
        private IUsableObject obj;

        private void Start()
        {
            obj = GetComponent(typeof(IUsableObject)) as IUsableObject;
        }

        public void Use()
        {
            use = true;
        }

        private void Update()
        {
            // Only update if object parent is the owner
            if (gameObject.transform.root.gameObject != owner)
            {
                return;
            }

            if (use)
            {
                use = false;
                obj.UseObject();
            }
        }
    }
}