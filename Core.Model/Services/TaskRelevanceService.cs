using Core.Model.Enums;

namespace Core.Model.Services;

public static class TaskRelevanceService
{
	public static Relevance Calculate(bool isFun, bool isProductive)
	{
		switch (isFun, isProductive)
		{
			case (false, false):
				return Relevance.Superficial;
			case (true, false):
				return Relevance.Entertaining;
			case (false, true):
				return Relevance.Necessary;
			case (true, true):
				return Relevance.Purposeful;
		}
	}
}
