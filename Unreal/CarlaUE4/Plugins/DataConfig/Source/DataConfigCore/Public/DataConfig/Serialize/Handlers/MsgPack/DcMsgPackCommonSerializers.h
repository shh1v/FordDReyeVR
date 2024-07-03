#pragma once
#include "CoreMinimal.h"
#include "DataConfig/DcTypes.h"
#include "DataConfig/Serialize/DcSerializeTypes.h"

namespace DcMsgPackHandlers
{

DATACONFIGCORE_API FDcResult HandlerMapSerialize(FDcSerializeContext& Ctx);

DATACONFIGCORE_API EDcSerializePredicateResult PredicateIsBlobProperty(FDcSerializeContext& Ctx);
DATACONFIGCORE_API FDcResult HandlerBlobSerialize(FDcSerializeContext& Ctx);
	
} // namespace DcMsgPackHandlers


