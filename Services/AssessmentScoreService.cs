using anisa_lms.DTOs;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace anisa_lms.Services
{
    public class AssessmentScoreService(
        IAssessmentScoreRepository repo,
        IAssessmentRepository assessmentRepo,
        IMapper mapper,
        IMemoryCache cache) : IAssessmentScoreService
    {
        private readonly IAssessmentScoreRepository _repo = repo;
        private readonly IAssessmentRepository _assessmentRepo = assessmentRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IMemoryCache _cache = cache;

        private const string ResultsKeyPrefix = "assessment_results_";

        private void InvalidateResultsCache(int assessmentId)
        {
            _cache.Remove($"{ResultsKeyPrefix}{assessmentId}_True");
            _cache.Remove($"{ResultsKeyPrefix}{assessmentId}_False");
        }

        public async Task CreateAssessmentScore(CreateAssessmentScoreDto create)
        {
            var assessment = await _assessmentRepo.GetByIdAsync(create.AssessmentId);
            if (assessment == null) throw new Exception("Assessment with given ID does not exist");

            var aScore = _mapper.Map<AssessmentScore>(create);

            await _repo.CreateAsync(aScore);
            await _repo.SaveChangesAsync();

            InvalidateResultsCache(create.AssessmentId);
        }

        public async Task<bool?> DeleteAssessmentScore(int asId)
        {
            var aScore = await _repo.GetByIdAsync(asId);
            if (aScore == null) return null;

            var assessmentId = aScore.AssessmentId;

            _repo.DeleteAsync(aScore);
            await _repo.SaveChangesAsync();

            InvalidateResultsCache(assessmentId);

            return true;
        }

        public async Task<bool?> UpdateAssessmentScore(int asId, UpdateAssessmentScoreDto update)
        {
            var aScore = await _repo.GetByIdAsync(asId);
            if (aScore == null) return null;

            _mapper.Map(update, aScore);
            await _repo.SaveChangesAsync();

            InvalidateResultsCache(aScore.AssessmentId);

            return true;
        }
    }
}
