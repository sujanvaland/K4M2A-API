using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class SubcriptionService : ISubcriptionService
    {
        private readonly IRepository<UserSubcription> _subcriptionRepository;

        public SubcriptionService(IRepository<UserSubcription> subcriptionRepository)
        {
            _subcriptionRepository = subcriptionRepository;
        }

        public async Task<JsonResponse> SaveSubcription(SubcriptionModel res, int userId)
        {
            UserSubcription subcription = new UserSubcription();
            subcription.UserId = userId;
            subcription.PlanId = res.PlanId;
            subcription.PaymentStatus = "completed";

            if (res.SubcriptionType == "individual")
            {
                subcription.IsIndividual = true;
                subcription.IsOrganization = false;
            }
            else
            {
                subcription.IsIndividual = false;
                subcription.IsOrganization = false;
            }

            if(res.SubcriptionType == "monthly")
            {
                subcription.IsMonthly = true;
                subcription.IsAnnual = false;
            }
            else
            {
                subcription.IsMonthly = false;
                subcription.IsAnnual = true;
            }

            await _subcriptionRepository.InsertAsync(subcription);
            return new JsonResponse(200, true, "Success", null);

        }
    }
}
