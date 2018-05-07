namespace SealTeam4
{
    public class UsableObject : InteractableObject
    {
        private bool use = false;
        private bool upButton = false;
        private bool downButton = false;
        private IUsableObject usableObj;
        private IButtonActivatable buttonActObj;

        private void Start()
        {
            usableObj = GetComponent(typeof(IUsableObject)) as IUsableObject;
            buttonActObj = GetComponent(typeof(IButtonActivatable)) as IButtonActivatable;
        }

        public void Use()
        {
            use = true;
        }

        public void UseUp()
        {
            upButton = true;
        }

        public void UseDown()
        {
            downButton = true;
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
                usableObj.UseObject();
            }

            if (buttonActObj != null)
            {
                CheckButtonPressed();
            }
        }

        private void CheckButtonPressed()
        {
            if (upButton)
            {
                upButton = false;
                buttonActObj.UpButtonPressed();
            }

            if (downButton)
            {
                downButton = false;
                buttonActObj.DownButtonPressed();
            }
        }
    }
}