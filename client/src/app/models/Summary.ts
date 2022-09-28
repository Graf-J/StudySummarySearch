export interface Summary {
    id?: number,
    url?: string,
    name: string,
    semester: number,
    subject: string,
    keywords: string[],
    author?: string,
    isImageLoading?: boolean
}