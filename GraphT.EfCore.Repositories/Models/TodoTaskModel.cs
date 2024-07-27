using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

namespace GraphT.EfCore.Repositories.Models;

public class TodoTaskModel : TaskAggregate
{
	public TodoTaskModel(
		string name, 
		bool isFun = false, 
		bool isProductive = false, 
		Complexity complexity = Complexity.Indefinite, 
		Priority priority = Priority.MentalClutter, 
		Status status = Status.Backlog) : base(name, isFun, isProductive, complexity, priority, status) { }
}
