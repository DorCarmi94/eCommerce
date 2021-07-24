namespace eCommerce.Service
{
    public class Services
    {
        public IAuthService AuthService { get; set; }
        public IUserService UserService { get; set; }
        public INStoreService InStoreService { get; set; }
        public ICartService CartService { get; set; }

        public Services(IAuthService authService, IUserService userService, 
            INStoreService inStoreService, ICartService cartService)
        {
            AuthService = authService;
            UserService = userService;
            InStoreService = inStoreService;
            CartService = cartService;
        }
    }
}