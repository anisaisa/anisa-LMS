using anisa_lms.DTOs;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using AutoMapper;

namespace anisa_lms.Services
{
    public class AssessmentScoreService(IAssessmentScoreRepository repo, IAssessmentRepository assessmentRepo, IMapper mapper) : IAssessmentScoreService
    {
        private readonly IAssessmentScoreRepository _repo = repo;
        private readonly IAssessmentRepository _assessmentRepo = assessmentRepo;
        private readonly IMapper _mapper = mapper;

        public async Task CreateAssessmentScore(CreateAssessmentScoreDto create)
        {
            var assessment = await _assessmentRepo.GetByIdAsync(create.AssessmentId);
            if (assessment == null) throw new Exception("Assessment with given ID does not exist");

            var aScore = _mapper.Map<AssessmentScore>(create);

            await _repo.CreateAsync(aScore);
            await _repo.SaveChangesAsync();
        }

        public async Task<bool?> DeleteAssessmentScore(int asId)
        {
            var aScore = await _repo.GetByIdAsync(asId);
            if (aScore == null) return null;

            _repo.DeleteAsync(aScore);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<bool?> UpdateAssessmentScore(int asId, UpdateAssessmentScoreDto update)
        {
            var aScore = await _repo.GetByIdAsync(asId);
            if (aScore == null) return null;

            _mapper.Map(update, aScore);
            await _repo.SaveChangesAsync();

            return true;
        }
    }
}
