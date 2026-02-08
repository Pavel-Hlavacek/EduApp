using AutoMapper;
using EduApp.Models;
using EduApp.ViewModels;
using QuizApp.Domain;

namespace EduApp.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Quiz, QuizDto>();
            CreateMap<QuizDto, Quiz>();

            CreateMap<Answer, AnswerDto>();
            CreateMap<AnswerDto, Answer>();

            CreateMap<Question, QuestionDto>();
            CreateMap<QuestionDto, Question>();
        }
    }
}
