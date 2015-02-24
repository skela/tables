//
//  TablePropertyScanner.h
//  tables
//
//  Created by Aleksander Slater on 08/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <Foundation/Foundation.h>

extern NSString *const TablePropertyScannerKey;
extern NSString *const TablePropertyScannerKeyType;
extern NSString *const TablePropertyScannerKeyClass;
extern NSString *const TablePropertyScannerKeyCell;
extern NSString *const TablePropertyScannerKeyTitle;
extern NSString *const TablePropertyScannerKeyPlaceholder;
extern NSString *const TablePropertyScannerKeyDefaultValue;
extern NSString *const TablePropertyScannerKeyOptions;
extern NSString *const TablePropertyScannerKeyTemplate;
extern NSString *const TablePropertyScannerKeyValueTransformer;
extern NSString *const TablePropertyScannerKeyAction;
extern NSString *const TablePropertyScannerKeySegue;
extern NSString *const TablePropertyScannerKeyHeader;
extern NSString *const TablePropertyScannerKeyFooter;
extern NSString *const TablePropertyScannerKeyInline;
extern NSString *const TablePropertyScannerKeySortable;
extern NSString *const TablePropertyScannerKeyViewController;

extern NSString *const TablePropertyScannerKeyTypeDefault;
extern NSString *const TablePropertyScannerKeyTypeLabel;
extern NSString *const TablePropertyScannerKeyTypeText;
extern NSString *const TablePropertyScannerKeyTypeLongText;
extern NSString *const TablePropertyScannerKeyTypeURL;
extern NSString *const TablePropertyScannerKeyTypeEmail;
extern NSString *const TablePropertyScannerKeyTypePhone;
extern NSString *const TablePropertyScannerKeyTypePassword;
extern NSString *const TablePropertyScannerKeyTypeNumber;
extern NSString *const TablePropertyScannerKeyTypeInteger;
extern NSString *const TablePropertyScannerKeyTypeUnsigned;
extern NSString *const TablePropertyScannerKeyTypeFloat;
extern NSString *const TablePropertyScannerKeyTypeBitfield;
extern NSString *const TablePropertyScannerKeyTypeBoolean;
extern NSString *const TablePropertyScannerKeyTypeOption;
extern NSString *const TablePropertyScannerKeyTypeDate;
extern NSString *const TablePropertyScannerKeyTypeTime;
extern NSString *const TablePropertyScannerKeyTypeDateTime;
extern NSString *const TablePropertyScannerKeyTypeImage;

@interface TablePropertyScanner : NSObject
+ (NSArray *)propertiesForObject:(id)obj;
@end
