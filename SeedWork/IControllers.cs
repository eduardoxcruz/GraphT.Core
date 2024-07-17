namespace SeedWork
{
    public interface IControllerTOutTIn<TOutputDto, TInputDto>
    {
        ValueTask<TOutputDto> RunUseCase(TInputDto inputDto);
    }

    public interface IControllerTIn<TInputDto>
    {
        ValueTask RunUseCase(TInputDto inputDto);
    }

    public interface IControllerTOut<TOutputDto>
    {
        ValueTask<TOutputDto> RunUseCase();
    }

    public interface IController
    {
        ValueTask RunUseCase();
    }
}
